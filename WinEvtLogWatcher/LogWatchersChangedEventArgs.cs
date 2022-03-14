using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    class LogWatchersChangedEventArgs : EventArgs
    {
        public IEnumerable<EventLogWatcherExtended> ChangedEventLogWatchers;
        public LogWatchersChangedEventArgs(IEnumerable<EventLogWatcherExtended> changedEventLogWatchers)
        {
            ChangedEventLogWatchers = changedEventLogWatchers;
        }
    }
}
