using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNV.LogWatcher.DataTransfer;

namespace TNV.LogWatcher.Agent
{
    class NewEventEntryCome : EventArgs
    {
        public EventEntry EventEntry { get; private set; }

        public NewEventEntryCome(EventEntry eventEntry)
        {
            EventEntry = eventEntry;
        }
    }
}
