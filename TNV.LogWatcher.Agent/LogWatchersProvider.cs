using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TNV.LogWatcher.DataTransfer;

namespace TNV.LogWatcher.Agent
{
    class LogWatchersProvider : ILogWatchersProvider
    {
        private readonly IDiagnosticsInformationProvider _diagInfoProvider;
        private List<EventLogWatcher> _cachedLogWatchers;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly string _queryString;
        private int _tryCreateCount;
        private readonly IEventLogsProvider _eventLogsService;
        public IEnumerable<EventLogWatcher> GetEventLogWatchers()
        {
            if (_cachedLogWatchers == null)
            {
                var logWatchers = new List<EventLogWatcher>();
                foreach (var eventLog in _eventLogsService.GetAvailableEventLogs())
                {
                    EventLogWatcher currentLogWatcher = null;
                    for (int i = 0; i < _tryCreateCount; i++)
                    {
                        currentLogWatcher = CreateLogWatcher(eventLog, _queryString);
                        if (currentLogWatcher != null)
                            break;
                    }
                    // все равно null, то пропускаем, да и хуй с ним
                    if (currentLogWatcher == null)
                        continue;

                    logWatchers.Add(currentLogWatcher);
                    _diagInfoProvider.AddMonitoredLogName(eventLog.Log);
                }
                _cachedLogWatchers = logWatchers;
            }

            return _cachedLogWatchers;
        }

        public LogWatchersProvider(IEventLogsProvider eventLogsService, IDiagnosticsInformationProvider diagInfoProvider, string queryString, int tryCreateCount)
        {
            _eventLogsService = eventLogsService ?? throw new ArgumentNullException(nameof(eventLogsService));
            _diagInfoProvider = diagInfoProvider ?? throw new ArgumentNullException(nameof(diagInfoProvider));
            _queryString = queryString;
            _tryCreateCount = tryCreateCount;
        }

        private EventLogWatcher CreateLogWatcher(EventLog eventLog, string queryString)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            EventLogWatcher eventLogWatcher = null;
            try
            {
                EventLogQuery eventLogQuery = new EventLogQuery(eventLog.Log, PathType.LogName, queryString);
                eventLogWatcher = new EventLogWatcher(eventLogQuery);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error creating event log watcher => LogName = {eventLog.Log}):\n{ex}");
            }

            stopWatch.Stop();
            _logger.Info($"Creating EventLogWatcher => LogName = {eventLog.Log}), elapsed time: {stopWatch.Elapsed}");

            return eventLogWatcher;
        }
    }
}
