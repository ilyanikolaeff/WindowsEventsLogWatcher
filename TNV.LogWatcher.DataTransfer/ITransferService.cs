using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TNV.LogWatcher.DataTransfer
{
    [ServiceContract]
    public interface ITransferService
    {
        [OperationContract]
        Task SendDiagnosticsInformation(DiagnosticsInformation data);

        [OperationContract]
        Task SendEvents(IEnumerable<EventEntry> events);


        event EventHandler<DiagInfoRecievedEventArgs> DiagInfoReceived;
        event EventHandler<EventsRecievedEventArgs> EventsReceived;
    }
}
