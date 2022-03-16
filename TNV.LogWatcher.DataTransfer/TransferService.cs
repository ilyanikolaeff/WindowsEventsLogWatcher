using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TNV.LogWatcher.DataTransfer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class TransferService : ITransferService
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        public event EventHandler<DiagInfoRecievedEventArgs> DiagInfoReceived;
        public event EventHandler<EventsRecievedEventArgs> EventsReceived;

        public Task SendDiagnosticsInformation(DiagnosticsInformation diagInfo)
        {
            _logger.Info(string.Format("Received diag info (time = {0}, session id = {1})",
                DateTime.Now.ToString("HH:mm:ss.ffffff"),
                OperationContext.Current.SessionId));
            OnDiagInfoReceived(diagInfo);
            return Task.CompletedTask;
        }

        public Task SendEvents(IEnumerable<EventEntry> events)
        {
            _logger.Info(string.Format("Received events (count = {0}, time = {1}, session id = {2})",
                events.Count(),
                DateTime.Now.ToString("HH:mm:ss.ffffff"),
                OperationContext.Current.SessionId));
            OnEventsReceived(events);
            return Task.CompletedTask;
        }

        private void OnDiagInfoReceived(DiagnosticsInformation diagInfo)
        {
            DiagInfoReceived?.Invoke(this, new DiagInfoRecievedEventArgs(diagInfo));
        }

        private void OnEventsReceived(IEnumerable<EventEntry> eventEntries)
        {
            EventsReceived?.Invoke(this, new EventsRecievedEventArgs(eventEntries));
        }
    }
}
