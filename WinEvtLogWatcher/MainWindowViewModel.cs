using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using NLog;
using System.Windows;
using System.Windows.Input;

namespace WinEvtLogWatcher
{
    class MainWindowViewModel : ViewModelBase
    {
        #region Services
        protected ICurrentWindowService CurrentWindowService { get { return GetService<ICurrentWindowService>(); } }
        protected INotifyIconService NotifyIconService { get { return GetService<INotifyIconService>(); } }
        #endregion

        public ICommand RestoreCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand ShowHistoryEvents { get; private set; }

        public bool IsAlwaysOnTop
        {
            get => Settings.GetInstance().IsAlwaysOnTop;
        }

        public MainWindowViewModel()
        {
            RestoreCommand = new DelegateCommand(() => RestoreWindow());
            CloseCommand = new DelegateCommand(() => CloseWindow());
            ShowHistoryEvents = new DelegateCommand(() => ShowHistoryWindow());
            Settings.GetInstance().SettingsChanged += SettingsChanged;
        }

        private void SettingsChanged(object sender, System.EventArgs e)
        {
            RaisePropertyChanged("IsAlwaysOnTop");
        }

        private void RestoreWindow()
        {
            CurrentWindowService.SetWindowState(WindowState.Normal);
            CurrentWindowService.Activate();
        }

        private void CloseWindow()
        {
            CurrentWindowService.Close();
        }

        private void ShowHistoryWindow()
        {
            DXMessageBox.Show("This feature is in development", "Information", MessageBoxButton.OK);
        }
    }
}
