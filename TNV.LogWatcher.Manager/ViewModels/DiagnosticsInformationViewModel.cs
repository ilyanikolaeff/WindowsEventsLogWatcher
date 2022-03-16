using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TNV.LogWatcher.DataTransfer;

namespace TNV.LogWatcher.Manager.ViewModels
{
    class DiagnosticsInformationViewModel : ViewModelBase
    {
        #region Fields
        private readonly ITransferService _transferService;
        #endregion

        #region Properties
        public ObservableCollection<DiagnosticsInformation> DiagnosticsInformation { get; set; } = new ObservableCollection<DiagnosticsInformation>();

        #endregion

        #region Commands

        #endregion

        #region Constructors
        public DiagnosticsInformationViewModel(ITransferService transferService)
        {
            _transferService = transferService ?? throw new ArgumentNullException(nameof(transferService));
            _transferService.DiagInfoReceived += OnDiagInfoReceived;
        }

        private void OnDiagInfoReceived(object sender, DiagInfoRecievedEventArgs e)
        {
            var exists = DiagnosticsInformation.FirstOrDefault(p => p.MachineName == e.DiagInfo.MachineName);
            if (exists != null)
                DiagnosticsInformation.Remove(exists);
            DiagnosticsInformation.Add(e.DiagInfo);
        }

        #endregion

        #region Methods
        #endregion
    }
}