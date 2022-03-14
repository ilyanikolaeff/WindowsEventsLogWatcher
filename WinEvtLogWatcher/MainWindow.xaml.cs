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
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static string layoutFileName = "Layout.xml";
        private Settings _settingsInstance;
        public MainWindow()
        {
            InitializeComponent();
            Width = SystemParameters.PrimaryScreenWidth * 0.67; // 2/3 width
            Height = SystemParameters.PrimaryScreenWidth * 0.3; // 1/3 height
            DataContext = new MainWindowViewModel();

            DXSplashScreen.SetState("[1/4] Инициализация настроек");

            try
            {
                _settingsInstance = Settings.GetInstance();
                if (_settingsInstance.UseDarkTheme)
                    ApplicationThemeHelper.ApplicationThemeName = Theme.VS2017DarkName;
                else
                    ApplicationThemeHelper.ApplicationThemeName = Theme.Office2019ColorfulName;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации настроек:" + Environment.NewLine + $"{ex}");
                Environment.Exit(0);
            }

            DXSplashScreen.SetState("[2/4] Инициализация сервисов");
            var loggerService = new ApplicationLogger();
            var historyService = new HistoryService(loggerService);
            var computersService = new ComputersService(loggerService);
            var logWatchersDiagnosticService = new LogWatcherDiagnosticService(new PingService(loggerService), loggerService);
            var eventLogWatcherService = new EventLogWatchersService(computersService, loggerService, logWatchersDiagnosticService);
            var timerService = new TimerService();
            //var soundPlayerService = new SoundPlayerService(loggerService);
            var soundPlayerService = new AlarmSoundPlayerService(loggerService);
            var eventsProvider = new EventsProvider(eventLogWatcherService, loggerService);

            DXSplashScreen.SetState("[3/4] Инициализация UI");
            var eventsGridViewModel = new EventsGridViewModel(eventsProvider, timerService, soundPlayerService, loggerService, historyService)
            {
                SubscribeStatus = true,
                LogSubscribersViewModel = new LogSubscribersViewModel(eventLogWatcherService),
                SettingsViewModel = new SettingsViewModel()
            };
            eventsGridUserControl.DataContext = eventsGridViewModel;

            DXSplashScreen.SetState("[4/4] Запуск сервисов в работу");
            eventsProvider.Start();
            timerService.Start();
            //Task.Run(async () => await soundPlayerService.Start());
            soundPlayerService.Start();
        }

        private void ThemedWindow_Closed(object sender, EventArgs e)
        {
            SaveLayout();
            Settings.Serialize(_settingsInstance);
            notifyIcon = null;
            //notifyIcon.Icon = null;
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
