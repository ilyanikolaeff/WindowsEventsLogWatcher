using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    class EventLogWatchersService : IEventLogWatchersService
    {
        private readonly IComputersService _computersService;
        private readonly IApplicationLogger _logger;
        private readonly Settings _settings;

        public EventLogWatchersService(IComputersService computersService, IApplicationLogger logger, Settings settings)
        {
            _computersService = computersService ?? throw new ArgumentNullException("computersService");
            _logger = logger ?? throw new ArgumentNullException("logger");
            _settings = settings ?? throw new ArgumentNullException("settings");
        }

        public IEnumerable<EventLogWatcherExtended> GetEventLogWatchers()
        {
            string queryString = _settings.QueryString;

            var logWatchers = new List<EventLogWatcherExtended>();
            var computers = _computersService.GetComputers();
            foreach (var computer in computers)
            {
                EventLog[] eventLogs = null;
                try
                {
                    eventLogs = computer.EventsLogs;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
                foreach (var eventLog in computer.EventsLogs)
                {
                    EventLogWatcherExtended eventLogWatcher = null;
                    try
                    {
                        EventLogQuery eventLogQuery = new EventLogQuery(eventLog.Log, PathType.LogName, queryString)
                        {
                            Session = new EventLogSession(computer.IpAddress)
                        };

                        eventLogWatcher = new EventLogWatcherExtended(eventLogQuery, eventLog.Log, computer, true) { Enabled = true };
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Error creating event log watcher:\n{ex}");
                    }
                    logWatchers.Add(eventLogWatcher);
                }
            }

            return logWatchers;
        }
    }
}
