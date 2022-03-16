using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TNV.LogWatcher.DataTransfer;

namespace TNV.LogWatcher.Agent
{
    class Program
    {
        static void Main(string[] args)
        {
            var configLoadResult = TryLoadConfiguration(out Configuration config);
            if (!configLoadResult)
                Environment.Exit(0);

            var transferService = GetTransferService(config);
            var eventLogsProvider = new EventLogsProvider();
            var diagInfoProvider = new DiagnosticsInformationProvider();

            // logs collector 
            var logCollectorService = new LogCollectorService(
                new LogMonitorService(
                    new LogWatchersProvider(
                        eventLogsProvider, diagInfoProvider, config.QueryString, config.TryCreateCount)), 
                transferService, 
                config.EventCollectionTime);

            // diga info collector service
            var diagInfoService = new DiagnosticsCollectorService(new DiagnosticsInformationProvider(), transferService, config.DiagInfoSendingInterval);

            Console.WriteLine("Starting finished");
            Console.ReadKey();
        }

        private static ILogger _logger = LogManager.GetCurrentClassLogger();

        private static bool TryLoadConfiguration(out Configuration config)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(Configuration));
                using (var streamReader = new StreamReader("Configuration.xml"))
                {
                    config = (Configuration)xmlSerializer.Deserialize(streamReader);
                    return true;
                }
            }
            catch (Exception ex)
            {
                config = null;
                _logger.Error(string.Format("Error loading config: {0}", ex));
                return false;
            }
        }

        private static ITransferService GetTransferService(Configuration configuration)
        {
            try
            {
                Uri tcpUri = new Uri($"net.tcp://{configuration.ServiceAddress}/{configuration.ServiceName}");
                EndpointAddress endpointAddress = new EndpointAddress(tcpUri);
                NetTcpBinding clientBinding = new NetTcpBinding();
                clientBinding.Security.Mode = SecurityMode.None;
                clientBinding.ReaderQuotas.MaxArrayLength = int.MaxValue;
                clientBinding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
                clientBinding.ReaderQuotas.MaxDepth = int.MaxValue;
                clientBinding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
                //clientBinding.TransferMode = TransferMode.Streamed;

                ChannelFactory<ITransferService> factory = new ChannelFactory<ITransferService>(clientBinding, endpointAddress);
                var service = factory.CreateChannel();
                return service;
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Error get service: {0}", ex));
                return null;
            }
        }
    }
}
