using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TNV.LogWatcher.DataTransfer;

namespace WcfClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceAddress = "127.0.0.1:65500";
            var serviceName = "LogWatcher";
            Uri tcpUri = new Uri($"net.tcp://{serviceAddress}/{serviceName}");
            EndpointAddress endpointAddress = new EndpointAddress(tcpUri);
            NetTcpBinding clientBinding = new NetTcpBinding();

            ChannelFactory<ITransferService> factory = new ChannelFactory<ITransferService>(clientBinding, endpointAddress);
            var service = factory.CreateChannel();

            int count = 0;
            while (true)
            {
                await service.SendDiagnosticsInformation(new DiagnosticsInformation() 
                { 
                    MachineName = "test machine name", 
                    OSVersion = "10.2.45"
                });
                var events = new List<EventEntry>();
                await service.SendEvents(events);

                Console.WriteLine($"Sended ({count++})");

                await Task.Delay(100);
            }
        }
    }
}
