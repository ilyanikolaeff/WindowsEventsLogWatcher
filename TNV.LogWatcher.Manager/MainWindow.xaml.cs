using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using System;
using System.Windows.Media;
using System.IO;
using System.Linq;
using System.Windows;
using NLog;
using TNV.LogWatcher.Manager.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using TNV.LogWatcher.DataTransfer;
using System.ServiceModel;

namespace TNV.LogWatcher.Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly string _layoutFileName = "Layout.xml";
        private readonly Settings _settings;
        public MainWindow()
        {
            InitializeComponent();

            DXSplashScreen.SetState("[1/4] Инициализация настроек");
            try
            {
                _settings = Settings.Deserialize();
                if (_settings.UseDarkTheme)
                    ApplicationThemeHelper.ApplicationThemeName = Theme.VS2017DarkName;
                else
                    ApplicationThemeHelper.ApplicationThemeName = Theme.Office2019ColorfulName;
            }
            catch (Exception ex)
            {
                DXSplashScreen.Close();
                MessageBox.Show($"Ошибка инициализации настроек:" + Environment.NewLine + $"{ex}");
                Environment.Exit(0);
            }

            DXSplashScreen.SetState("[2/4] Инициализация сервисов");
            var transferService = HostTransferService(_settings);
            if (transferService == null)
            {
                MessageBox.Show($"Не удалось захостить WCF");
                Environment.Exit(0);
            }
            var historyService = new HistoryService();
            var timerService = new TimerService(_settings);
            var soundPlayerService = new AlarmSoundPlayerService(_settings);

            DXSplashScreen.SetState("[3/4] Инициализация UI");
            var eventsGridViewModel = new EventsGridViewModel(transferService, timerService, soundPlayerService, historyService, _settings)
            {
                SubscribeStatus = true,
                DiagnosticsViewModel = new DiagnosticsInformationViewModel(transferService),
                SettingsViewModel = new SettingsViewModel(_settings)
            };
            eventsGridUserControl.DataContext = eventsGridViewModel;

            Width = SystemParameters.PrimaryScreenWidth * 0.67; // 2/3 width
            Height = SystemParameters.PrimaryScreenWidth * 0.3; // 1/3 height
            DataContext = new MainWindowViewModel(_settings);


            DXSplashScreen.SetState("[4/4] Запуск сервисов в работу");
            timerService.Start();
            soundPlayerService.Start();
        }

        private void ThemedWindow_Closed(object sender, EventArgs e)
        {
            SaveLayout();
            Settings.Serialize(_settings);
            notifyIcon = null;
        }

        private void ThemedWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RestoreLayout();
            DXSplashScreen.Close();
        }


        private void SaveLayout()
        {
            eventsGridUserControl.eventsGridControl.SaveLayoutToXml(_layoutFileName);
        }

        private void RestoreLayout()
        {
            if (File.Exists(_layoutFileName))
            {
                eventsGridUserControl.eventsGridControl.RestoreLayoutFromXml(_layoutFileName);
            }
        }

        private void ThemedWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                ShowInTaskbar = false;

            if (WindowState == WindowState.Normal || WindowState == WindowState.Maximized)
                ShowInTaskbar = true;

        }

        private ITransferService HostTransferService(Settings settings)
        {
            try
            {
                var transferService = new TransferService();
                var serviceAddress = settings.ServiceAddress;
                var serviceName = settings.ServiceName;
                //var host = new ServiceHost(typeof(TransferContract), new Uri($"net.tcp://{serviceAddress}/{serviceName}"));
                var host = new ServiceHost(transferService, new Uri($"net.tcp://{serviceAddress}/{serviceName}"));
                var serverBinding = new NetTcpBinding();
                serverBinding.Security.Mode = SecurityMode.None;
                serverBinding.ReaderQuotas.MaxArrayLength = int.MaxValue;
                serverBinding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
                serverBinding.ReaderQuotas.MaxDepth = int.MaxValue;
                serverBinding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
                //serverBinding.TransferMode = TransferMode.Streamed;

                host.AddServiceEndpoint(typeof(ITransferService), serverBinding, "");
                host.Open();
                _logger.Info($"Binding hosted. Address = {host.BaseAddresses[0]}");

                return transferService;
            }
            catch (Exception ex)
            {
                DXSplashScreen.Close();
                _logger.Error(ex);
                return null;
            }
        }
    }
}
