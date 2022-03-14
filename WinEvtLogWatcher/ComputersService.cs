using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    class ComputersService : IComputersService
    {
        private readonly Settings _settingsProvider = Settings.GetInstance();
        private readonly IApplicationLogger _logger;
        private IEnumerable<Computer> _cachedComputers = null;


        public IEnumerable<Computer> GetComputers()
        {
            if (_cachedComputers == null)
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();

                var computersList = new List<Computer>();
                foreach (var host in _settingsProvider.HostSettings)
                {
                    var currentComp = GetComputer(host.IpAddress, host.Domain, host.User, host.Password);
                    if (currentComp != null)
                        computersList.Add(currentComp);
                }

                stopWatch.Stop();
                _logger.Info($"GetComputers() elapsed time: {stopWatch.Elapsed}");
                _cachedComputers = computersList;
            }
            return _cachedComputers;
        }

        public ComputersService(IApplicationLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException("logger");
        }

        private Computer GetComputer(string ipAddress, string domain, string user, string password)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            Computer currentComp = null;

            string currentMachineName = ipAddress;
            try
            {
                currentMachineName = RunWithTimeout(() => GetMachineName(ipAddress), TimeSpan.FromSeconds(5));
            }
            catch (Exception ex)
            {
                _logger.Error($"GetMachineName for {ipAddress} error: {ex}");
            }

            try
            {
                EventLog[] currentEventLogs = RunWithTimeout(() => GetAvailableEventsLogs(ipAddress), TimeSpan.FromSeconds(5));
                currentComp = new Computer(ipAddress, domain, user, password, currentMachineName, currentEventLogs);

                _logger.Info($"Creating <Computer> instance for {ipAddress} elapsed time: {stopWatch.Elapsed}");
                stopWatch.Stop();

                return currentComp;
            }

            catch (Exception ex)
            {
                _logger.Error($"Creating <Computer> instance for {ipAddress} error: {ex}");
            }
            return currentComp;
        }

        private string GetMachineName(string ipAddress)
        {
            string machineName = "";
            try
            {
                var hostEntry = Dns.GetHostEntry(ipAddress);
                machineName = hostEntry.HostName;
            }
            catch (Exception ex)
            {
                _logger.Error($"Cannt get host name of [{ipAddress}]. Exception:\n{ex}");
            }
            return machineName;
        }
        private EventLog[] GetAvailableEventsLogs(string ipAddress)
        {
            EventLog[] eventLogs = new EventLog[0];
            try
            {
                eventLogs = EventLog.GetEventLogs(ipAddress);
            }
            catch (Exception ex)
            {
                _logger.Error($"Cannt get event logs of [{ipAddress}]. Exception:\n{ex}");
            }
            return eventLogs;
        }

        private T RunWithTimeout<T>(Func<T> action, TimeSpan timeout)
        {
            var task = Task.Run(action);
            var success = task.Wait(timeout);
            if (!success)
            {
                throw new TimeoutException();
            }
            else
            {
                return task.Result;
            }
        }
    }
}
