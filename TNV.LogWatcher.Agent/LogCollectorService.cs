using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TNV.LogWatcher.DataTransfer;

namespace TNV.LogWatcher.Agent
{
    class LogCollectorService : ILogCollectorService
    {
        private readonly object _locker = new object();
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly List<EventEntry> _entries = new List<EventEntry>();
        private readonly Timer _collectTimer;
        private readonly ILogMonitorService _logMonitorService;
        private readonly ITransferService _transferService;
        public LogCollectorService(ILogMonitorService logMonitorService, ITransferService transferService, int collectTime)
        {
            _logMonitorService = logMonitorService ?? throw new ArgumentNullException();
            _logMonitorService.NewEventEntryCome += OnNewEventRecordCome;

            _transferService = transferService ?? throw new ArgumentNullException(nameof(transferService));

            _collectTimer = new Timer(collectTime)
            {
                AutoReset = true,
                Enabled = true
            };
            _collectTimer.Elapsed += OnCollectTimerElapsed;
        }

        private void OnNewEventRecordCome(object sender, NewEventEntryCome e)
        {
            lock (_locker)
            {
                _entries.Add(e.EventEntry);
                _logger.Info($"New event entry added. Count = {_entries.Count}");
            }
        }

        private async void OnCollectTimerElapsed(object sender, ElapsedEventArgs e)
        {
            List<EventEntry> events = null;
            lock (_locker)
            {
                events = new List<EventEntry>(_entries);
                _entries.Clear();
            }
            if (events != null && events.Count > 0)
            {
                await _transferService.SendEvents(events);
                _logger.Info($"Events (count = {events.Count}) sended!");
            }
        }
    }
}
