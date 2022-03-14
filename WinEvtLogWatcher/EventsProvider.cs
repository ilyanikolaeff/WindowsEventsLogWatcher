using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    class EventsProvider : IEventsProvider
    {
        private readonly IEventLogWatchersService _eventLogWatchersService;
        private readonly IApplicationLogger _logger;
        private ConcurrentBag<EventLogWatcherExtended> _watchers;

        public EventsProvider(IEventLogWatchersService eventLogWatchersService, IApplicationLogger logger)
        {
            _eventLogWatchersService = eventLogWatchersService ?? throw new ArgumentNullException("eventLogWatcherService");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventLogWatchersService.LogWatchersChanged += OnLogWatchersChanged;
            _eventLogWatchersService.ComputersDisconnected += OnComputersDisconnected;
            _watchers = new ConcurrentBag<EventLogWatcherExtended>(_eventLogWatchersService.GetEventLogWatchers());
        }

        private void OnComputersDisconnected(object sender, ComputerDisconnectedEventArgs e)
        {
            if (e != null && e.DisconnectedComputers != null)
            {
                foreach (var discComputer in e.DisconnectedComputers)
                {
                    foreach (var discLogwatcher in _watchers.Where(p => p.Computer == discComputer))
                    {
                        discLogwatcher.Enabled = false;
                        discLogwatcher.EventRecordWritten -= OnNewRecordCome;
                    }
                }
            }
        }

        private void OnLogWatchersChanged(object sender, LogWatchersChangedEventArgs e)
        {
            _logger.Info($"Processing log watchers changed event");
            Stop();
            ClearLogWatchers();
            _eventLogWatchersService.GetEventLogWatchers().ToList().ForEach(a => _watchers.Add(a));
            Start();
        }

        public void Start()
        {
            SetEnabled(true);
        }

        public void Stop()
        {
            SetEnabled(false);
        }


        private void ClearLogWatchers()
        {
            while (_watchers.Count > 0)
                _watchers.TryTake(out EventLogWatcherExtended tmp);
        }

        private void SetEnabled(bool value)
        {
            _logger.Info($"Set Enabled = {value} for {_watchers.Count()} eventLogWatchers");
            foreach (var logWatcher in _watchers)
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
            try
            {
                if (e.EventRecord != null)
                {
                    var eventEntry = new EventEntry(e.EventRecord);
                    NewEventEntryAppeared?.Invoke(this, new NewEventEntryComeEventArgs(eventEntry));
                }
            }
            catch
            {
                //_logger.Error($"Error processing new event record:\n{ex}");
            }
        }

        public event EventHandler<NewEventEntryComeEventArgs> NewEventEntryAppeared;
    }
}
