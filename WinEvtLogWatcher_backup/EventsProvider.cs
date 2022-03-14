using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    class EventsProvider : IEventsProvider
    {
        private readonly IEventLogWatchersService _eventLogWatchersService;
        public EventsProvider(IEventLogWatchersService eventLogWatchersService)
        {
            _eventLogWatchersService = eventLogWatchersService ?? throw new ArgumentNullException("eventLogWatcherService");
        }

        public void Start()
        {
            SetEnabled(true);
        }

        public void Stop()
        {
            SetEnabled(false);
        }

       
        private void SetEnabled(bool value)
        {
            foreach (var logWatcher in _eventLogWatchersService.GetEventLogWatchers())
            {
                logWatcher.Enabled = value;
                if (value)
                    logWatcher.EventRecordWritten += OnNewRecordCome;
                else
                    logWatcher.EventRecordWritten -= OnNewRecordCome;
            }
        }

        private void OnNewRecordCome(object sender, System.Diagnostics.Eventing.Reader.EventRecordWrittenEventArgs e)
        {
            if (e.EventRecord != null)
            {
                NewEventEntryAppeared?.Invoke(this, new NewEventEntryComeEventArgs(new EventEntry(e.EventRecord)));
            }
        }

        public event EventHandler<NewEventEntryComeEventArgs> NewEventEntryAppeared;
    }
}
