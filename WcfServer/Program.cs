using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TNV.LogWatcher.DataTransfer;

namespace WcfServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceAddress = "127.0.0.1:65500";
            var serviceName = "LogWatcher";
            //var host = new ServiceHost(typeof(TransferContract), new Uri($"net.tcp://{serviceAddress}/{serviceName}"));
            var host = new ServiceHost(new TransferService(), new Uri($"net.tcp://{serviceAddress}/{serviceName}"));
            var serverBinding = new NetTcpBinding();
            serverBinding.Security.Mode = SecurityMode.None;
            serverBinding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            serverBinding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            serverBinding.ReaderQuotas.MaxDepth = int.MaxValue;
            serverBinding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            //serverBinding.TransferMode = TransferMode.Streamed;

            host.AddServiceEndpoint(typeof(ITransferService), serverBinding, "");
            host.Open();
            Console.WriteLine($"Binding hosted. Address = {host.BaseAddresses[0]}");
            Console.ReadKey();
        }
    }
}
