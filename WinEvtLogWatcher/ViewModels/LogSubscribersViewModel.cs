using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WinEvtLogWatcher.ViewModels
{
    class LogSubscribersViewModel : ViewModelBase
    {
        #region Fields
        private IEventLogWatchersService _logWatcherService;
        #endregion

        #region Properties
        public List<EventLogWatcherExtended> LogWatchers
        {
            get => GetValue<List<EventLogWatcherExtended>>();
            set => SetValue(value);
        }

        public EventLogWatcherExtended SelectedEventLogWatcher
        {
            get => GetValue<EventLogWatcherExtended>();
            set => SetValue(value);
        }

        #endregion

        #region Commands
        //public DelegateCommand AddLogSubscriberCommand
        //{
        //    get
        //    {
        //        return new DelegateCommand(
        //        () =>
        //        {
        //            var addComputerDlgWindow = new AddComputerDialog();
        //            if (addComputerDlgWindow.ShowDialog() == true)
        //            {
        //                if (Settings.GetInstance().IpAddresses.Contains(addComputerDlgWindow.IpAddress))
        //                {
        //                    DXMessageBox.Show("Такой компьютер уже существует", "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
        //                }
        //                else
        //                {
        //                    AddNewLogSubscriber(addComputerDlgWindow.IpAddress);

        //                    // Дополнительно в настройки еще записываем
        //                    Settings.GetInstance().IpAddresses.Add(addComputerDlgWindow.IpAddress);
        //                }
        //            }
        //        });
        //    }
        //}
        //public DelegateCommand RemoveLogSubscriberCommand
        //{
        //    get
        //    {
        //        return new DelegateCommand(
        //        () =>
        //        {
        //            // Дополнительно удаляем из настроек
        //            Settings.GetInstance().IpAddresses.Remove(SelectedLogSubscriber.Computer.IpAddress);

        //            RemoveLogSubscriber(SelectedLogSubscriber);
        //        },
        //        () => SelectedLogSubscriber != null);
        //    }
        //}
        #endregion

        #region Constructors
        public LogSubscribersViewModel(IEventLogWatchersService logWatcherService)
        {
            _logWatcherService = logWatcherService ?? throw new ArgumentNullException(nameof(logWatcherService));
            _logWatcherService.LogWatchersChanged += OnLogWatchersChanged;

            SetLogWatchers(_logWatcherService.GetEventLogWatchers());
        }

        private void OnLogWatchersChanged(object sender, LogWatchersChangedEventArgs e)
        {
            SetLogWatchers(_logWatcherService.GetEventLogWatchers());
        }

        private void SetLogWatchers(IEnumerable<EventLogWatcherExtended> logWatchers)
        {
            LogWatchers = new List<EventLogWatcherExtended>(logWatchers);
        }
        #endregion

        #region Methods
        #endregion
    }
}