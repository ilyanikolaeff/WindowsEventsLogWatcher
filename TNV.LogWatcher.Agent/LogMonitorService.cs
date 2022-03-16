using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNV.LogWatcher.DataTransfer;

namespace TNV.LogWatcher.Agent
{
    class LogMonitorService : ILogMonitorService
    {
        private readonly ILogWatchersProvider _logWatcherService;
        public LogMonitorService(ILogWatchersProvider logWatchersService)
        {
            _logWatcherService = logWatchersService ?? throw new ArgumentNullException(nameof(logWatchersService));
            foreach (var logWatcher in _logWatcherService.GetEventLogWatchers())
            {
                logWatcher.EventRecordWritten += OnEventRecordWritten;
                logWatcher.Enabled = true;
            }
        }

        public event EventHandler<NewEventEntryCome> NewEventEntryCome;

        private void OnEventRecordWritten(object sender, EventRecordWrittenEventArgs e)
        {
            if (e.EventRecord != null)
            {
                var eventEntry = new EventEntry(e.EventRecord);
                NewEventEntryCome?.Invoke(this, new NewEventEntryCome(eventEntry));
            }   
        }
    }
}
