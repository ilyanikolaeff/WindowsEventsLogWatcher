using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    class EventLogWatchersService : IEventLogWatchersService
    {
        private readonly IComputersService _computersService;
        private readonly IApplicationLogger _logger;
        private readonly ILogWatcherDiagnosticService _logWatcherDiagnosticService;
        private System.Timers.Timer _monitoringTimer;

        private readonly Settings _settings = Settings.GetInstance();

        private List<EventLogWatcherExtended> _cachedEventLogWatchers = null;

        public event EventHandler<LogWatchersChangedEventArgs> LogWatchersChanged;
        public event EventHandler<ComputerDisconnectedEventArgs> ComputersDisconnected;

        public EventLogWatchersService(IComputersService computersService, IApplicationLogger logger, ILogWatcherDiagnosticService logWatcherDiagnosticService)
        {
            _computersService = computersService ?? throw new ArgumentNullException("computersService");
            _logger = logger ?? throw new ArgumentNullException("logger");
            _logWatcherDiagnosticService = logWatcherDiagnosticService ?? throw new ArgumentNullException(nameof(logWatcherDiagnosticService));
            StartMonitoring();
        }

        public IEnumerable<EventLogWatcherExtended> GetEventLogWatchers()
        {
            if (_cachedEventLogWatchers == null)
            {
                var logWatchers = new List<EventLogWatcherExtended>();
                var computers = _computersService.GetComputers();
                foreach (var computer in computers)
                {
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    var currCompLogWatchers = CreateLogWatchers(computer);
                    logWatchers.AddRange(currCompLogWatchers);

                    _logger.Info($"GetEventLogWatchers() (count = {currCompLogWatchers.Count()}, CompIp = {computer.IpAddress}) elapsed time: {stopWatch.Elapsed}");
                }
                _cachedEventLogWatchers = logWatchers;
            }

            return _cachedEventLogWatchers;
        }

        private EventLogWatcherExtended CreateLogWatcher(EventLog eventLog, Computer computer, string queryString)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            EventLogWatcherExtended eventLogWatcher = null;
            try
            {
                EventLogQuery eventLogQuery = new EventLogQuery(eventLog.Log, PathType.LogName, queryString)
                {
                    Session = GetEventLogSession(computer)
                };

                eventLogWatcher = new EventLogWatcherExtended(eventLogQuery, eventLog.Log, computer, true) { Enabled = true };
            }
            catch (Exception ex)
            {
                _logger.Error($"Error creating event log watcher (CompIp = {computer.IpAddress}, LogName = {eventLog.Log}):\n{ex}");
            }

            stopWatch.Stop();
            _logger.Info($"Creating EventLogWatcher (CompIp = {computer.IpAddress}, LogName = {eventLog.Log}) elapsed time: {stopWatch.Elapsed}");

            return eventLogWatcher;
        }

        private IEnumerable<EventLogWatcherExtended> CreateLogWatchers(Computer computer)
        {
            var logWatchers = new List<EventLogWatcherExtended>();
            var excludedJournalNames = _settings.HostSettings.FirstOrDefault(p => p.IpAddress == computer.IpAddress).ExcludingJournalNames;
            foreach (var eventLog in computer.EventsLogs)
            {
                if (excludedJournalNames.Contains(eventLog.Log))
                    continue;

                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                EventLogWatcherExtended currentLogWatcher = null;

                // с первого раза создать не удалось, пробуем пересоздать (неск. раз)
                for (int i = 0; i < _settings.TryRecreateLogWatcherCount + 1; i++)
                {
                    currentLogWatcher = CreateLogWatcher(eventLog, computer, _settings.QueryString);
                    if (currentLogWatcher != null)
                        break;
                }
                // все равно null, то пропускаем, да и хуй с ним
                if (currentLogWatcher == null)
                    continue;

                logWatchers.Add(currentLogWatcher);
            }
            return logWatchers;
        }

        private SecureString ConvertToSecureString(string password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            var secureString = new SecureString();

            foreach (var c in password)
                secureString.AppendChar(c);

            secureString.MakeReadOnly();
            return secureString;
        }

        private EventLogSession GetEventLogSession(Computer computer)
        {
            string ipAddress = computer.IpAddress;
            string domain = computer.Domain;
            string user = computer.User;
            string password = computer.Password;

            if (string.IsNullOrEmpty(user))
                return new EventLogSession(ipAddress);
            else
                return new EventLogSession(ipAddress, domain, user, ConvertToSecureString(password), SessionAuthentication.Default);
        }

        public void StartMonitoring()
        {
            _monitoringTimer = new System.Timers.Timer(_settings.DiagnosePeriod * 1000);
            _monitoringTimer.Enabled = true;
            _monitoringTimer.Elapsed += OnMonitoringTimerElapsed;
        }

        private async void OnMonitoringTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _logger.Info($"Calling diagnostic connection to computers");
            try
            {
                var results = await _logWatcherDiagnosticService.Diagnose(_computersService.GetComputers());

                // disconnected computers
                var discComputers = results.Where(p => !p.IsConnected).Select(s => s.Computer);
                await Task.Run(() => UnsubComputers(discComputers));

                // resub
                var resubscribeComputers = results.Where(p => p.IsNeedResub).Select(s => s.Computer);
                await ResubComputers(resubscribeComputers);


            }
            catch (Exception ex)
            {
                _logger.Error($"Diagnostic connection failed:\n{ex}");
            }
        }

        private void UnsubComputers(IEnumerable<Computer> discComputers)
        {
            _logger.Info($"Diagnostic connection results: disconnectedComputers = [{discComputers.Count()}]");
            ComputersDisconnected?.Invoke(this, new ComputerDisconnectedEventArgs(discComputers));
        }

        private async Task ResubComputers(IEnumerable<Computer> resubscribeComputers)
        {
            _logger.Info($"Diagnostic connection results: computers to resub = [{resubscribeComputers.Count()}]");
            if (resubscribeComputers.Count() > 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(_settings.DelayAfterBadDiagnose));

                var recreatedEventLogWatchers = new List<EventLogWatcherExtended>();
                foreach (var resubComp in resubscribeComputers)
                {
                    _logger.Info($"Recreating log watchers for {resubComp}");
                    recreatedEventLogWatchers.AddRange(CreateLogWatchers(resubComp));
                }

                // если есть кешированые, то заменяем там пересозданные
                if (_cachedEventLogWatchers != null)
                {
                    foreach (var comp in resubscribeComputers)
                        _cachedEventLogWatchers.RemoveAll(p => p.Computer == comp);
                    _cachedEventLogWatchers.AddRange(recreatedEventLogWatchers);
                }

                // Взводим event
                LogWatchersChanged?.Invoke(this, new LogWatchersChangedEventArgs(recreatedEventLogWatchers));
            }
        }

    }
}
