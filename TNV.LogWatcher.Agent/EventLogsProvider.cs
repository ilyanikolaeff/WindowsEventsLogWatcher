using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNV.LogWatcher.Agent
{
    class EventLogsProvider : IEventLogsProvider
    {
        private IEnumerable<EventLog> _cachedEventLogs;
        public IEnumerable<EventLog> GetAvailableEventLogs()
        {
            if (_cachedEventLogs == null)
            {
                _cachedEventLogs = EventLog.GetEventLogs();
            }
            return _cachedEventLogs;
        }
    }
}
