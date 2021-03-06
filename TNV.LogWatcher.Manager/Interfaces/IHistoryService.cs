using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNV.LogWatcher.DataTransfer;

namespace TNV.LogWatcher.Manager
{
    interface IHistoryService
    {
        void SaveEntry(EventEntry eventEntry);
        Task ExportAlarmsToHistory(IEnumerable<EventEntry> events);
    }
}
