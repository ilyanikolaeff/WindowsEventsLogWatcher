using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TNV.LogWatcher.DataTransfer;

namespace TNV.LogWatcher.Agent
{
    class DiagnosticsCollectorService : IDiagnosticsCollectorService
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly ITransferService _transferService;
        private readonly Timer _sendingTimer;
        private readonly IDiagnosticsInformationProvider _diagInfoProvider;
        public DiagnosticsCollectorService(IDiagnosticsInformationProvider diagInfoProvider, ITransferService transferService, int sendingInterval)
        {
            _transferService = transferService ?? throw new ArgumentNullException(nameof(transferService));
            _diagInfoProvider = diagInfoProvider ?? throw new ArgumentNullException(nameof(diagInfoProvider));

            _sendingTimer = new Timer(sendingInterval)
            {
                AutoReset = true,
                Enabled = true
            };
            _sendingTimer.Elapsed += OnSendingTimerElapsed;
        }

        private async void OnSendingTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var diagInfo = _diagInfoProvider.GetDiagnosticsInformation();
            await _transferService.SendDiagnosticsInformation(diagInfo);
            _logger.Info($"DiagInfo sended: {DateTime.Now:HH:mm:ss.fffffff}");
        }


    }
}
