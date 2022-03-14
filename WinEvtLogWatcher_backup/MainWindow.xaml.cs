using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using System;
using System.Windows.Media;
using System.IO;
using System.Linq;
using System.Windows;
using NLog;
using WinEvtLogWatcher.ViewModels;
using System.Collections.Generic;

namespace WinEvtLogWatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static string layoutFileName = "Layout.xml";
        private Settings _settingsInstance;
        private SettingsLoader _settingsLoader;
        public MainWindow()
        {
            InitializeComponent();
            MaxWidth = SystemParameters.PrimaryScreenWidth;
            MaxHeight = SystemParameters.PrimaryScreenWidth;
            DataContext = new MainWindowViewModel();

            DXSplashScreen.SetState("[1/3] Инициализация настроек");
            _settingsLoader = new SettingsLoader();
            _settingsInstance = _settingsLoader.LoadSettings();


            if (_settingsInstance.UseDarkTheme)
                ApplicationThemeHelper.ApplicationThemeName = Theme.VS2017DarkName;
            else
                ApplicationThemeHelper.ApplicationThemeName = Theme.Office2019ColorfulName;

            DXSplashScreen.SetState("[2/3] Инициализация сервисов");
            var loggerService = new ApplicationLogger();
            var computersService = new ComputersService(_settingsInstance);
            var eventLogWatcherService = new EventLogWatchersService(computersService, loggerService, _settingsInstance);
            var exportService = new ExportService();
            var timerService = new TimerService(_settingsInstance);
            var soundPlayerService = new SoundPlayerService(_settingsInstance);
            var eventsProvider = new EventsProvider(eventLogWatcherService);

            DXSplashScreen.SetState("[3/3] Инициализация UI");
            var eventsGridViewModel = new EventsGridViewModel(eventsProvider, exportService, _settingsInstance, timerService, soundPlayerService, loggerService);
            eventsGridUserControl.DataContext = eventsGridViewModel;

            var logSubscribersViewModel = new LogSubscribersViewModel(eventLogWatcherService);
            logSubscribersUserControl.DataContext = logSubscribersViewModel;

            var settingsViewModel = new SettingsViewModel(_settingsInstance);
            settingsUserControl.DataContext = settingsViewModel;

            eventsProvider.Start();
        }

        private void ThemedWindow_Closed(object sender, EventArgs e)
        {
            SaveLayout();
            _settingsLoader.SaveSettings(_settingsInstance);
        }

        private void ThemedWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RestoreLayout();
            DXSplashScreen.Close();
        }


        private void SaveLayout()
        {
            eventsGridUserControl.eventsGridControl.SaveLayoutToXml(layoutFileName);
        }

        private void RestoreLayout()
        {
            if (File.Exists(layoutFileName))
            {
                eventsGridUserControl.eventsGridControl.RestoreLayoutFromXml(layoutFileName);
            }
        }

        private void ThemedWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                ShowInTaskbar = false;

            if (WindowState == WindowState.Normal || WindowState == WindowState.Maximized)
                ShowInTaskbar = true;

        }
    }
}
