using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    class PingService
    {
        private readonly IApplicationLogger _logger;
        public PingService(IApplicationLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> TryPingHostAsync(string hostNameOrAddress)
        {
            Ping pinger = new Ping();
            var pingReply = await pinger.SendPingAsync(hostNameOrAddress);
            if (pingReply.Status != IPStatus.Success)
            {
                _logger.Info($"[{nameof(PingService)}] Ip = {hostNameOrAddress}, pingReply = {pingReply.Status}");
            }    
            return pingReply.Status == IPStatus.Success;
        }
    }
}
