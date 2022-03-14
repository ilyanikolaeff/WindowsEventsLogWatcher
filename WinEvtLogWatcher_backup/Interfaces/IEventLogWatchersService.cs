using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    interface IEventLogWatchersService
    {
        IEnumerable<EventLogWatcherExtended> GetEventLogWatchers();
    }
}
