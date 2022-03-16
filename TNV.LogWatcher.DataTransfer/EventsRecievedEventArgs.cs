using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNV.LogWatcher.DataTransfer
{
    public class EventsRecievedEventArgs
    {
        public IEnumerable<EventEntry> EventEntries { get; }
        public EventsRecievedEventArgs(IEnumerable<EventEntry> eventEntries)
        {
            EventEntries = eventEntries;
        }
    }
}
