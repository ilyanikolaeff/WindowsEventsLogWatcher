using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    interface IHistoryService
    {
        void SaveEntry(EventEntry eventEntry);
        Task ExportAlarmsToHistory(IEnumerable<EventEntry> events);
    }
}
