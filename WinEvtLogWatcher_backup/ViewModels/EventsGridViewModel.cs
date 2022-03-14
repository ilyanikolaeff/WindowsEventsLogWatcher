using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WinEvtLogWatcher.ViewModels
{
    class EventsGridViewModel : ViewModelBase
    {
        #region Fields
        private readonly IEventsProvider _eventsProvider;
        private readonly IExportService _exportService;
        private readonly ITimerService _timerService;
        private readonly Settings _settingsProvider;
        private readonly ISoundPlayerService _soundPlayerService;
        private readonly IApplicationLogger _logger;
        #endregion

        #region Properties
        public ObservableCollection<EventEntry> Events { get; set; } = new ObservableCollection<EventEntry>();
        public bool SubscribeStatus 
        { 
            get => GetValue<bool>(); 
            set => SetValue(value); 
        }
        public EventEntry CurrentItem
        {
            get { return GetValue<EventEntry>(); }
            set { SetValue(value); }
        }
        #endregion

        #region Constructors
        public EventsGridViewModel(IEventsProvider eventsProvider, IExportService exportService, Settings settingsProvider, ITimerService timerService,
            ISoundPlayerService soundPlayerService, IApplicationLogger logger)
        {
            _eventsProvider = eventsProvider ?? throw new ArgumentNullException("eventsProvider");
            _exportService = exportService ?? throw new ArgumentNullException("exportService");
            _settingsProvider = settingsProvider ?? throw new ArgumentNullException("settingsProvider");
            _timerService = timerService ?? throw new ArgumentNullException("timerService");
            _soundPlayerService = soundPlayerService ?? throw new ArgumentNullException("soundPlayerService");
            _logger = logger ?? throw new ArgumentNullException("logger");

            _eventsProvider.NewEventEntryAppeared += OnNewEventEntryCome;
            _timerService.TimerElapsed += OnTimerElapsed;

            SetCommands();
        }

        #endregion

        #region Methods
        private Task ExportCurrentEvents(bool needClear)
        {
            try
            {
                if (Application.Current != null)
                {
                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        var currentEvents = Events.ToList();
                        if (needClear)
                            Events.Clear();
                        await _exportService.ExportAlarmsToHistory(currentEvents);
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error export events (needClear = {needClear}):\n{ex}");
            }

            return Task.CompletedTask;
        }

        private void OnTimerElapsed(object sender, EventArgs e)
        {
            ExportCurrentEvents(true);
        }
        private void OnNewEventEntryCome(object sender, NewEventEntryComeEventArgs e)
        {
            if (e.NewEventEntry != null)
            {
                AddEventToTableView(e.NewEventEntry);
                Task.Run(() => AddEventToSoundPlayer(e.NewEventEntry));
            }
        }
        private void AddEventToTableView(EventEntry eventRecord)
        {
            try
            {
                if (Application.Current != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (Events.Count > _settingsProvider.MaxEventsToDisplay)
                            ExportCurrentEvents(true);

                        Events.Add(eventRecord);
                        CurrentItem = eventRecord;
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error add event to table view:\n{ex}");
            }
        }

        private async Task AddEventToSoundPlayer(EventEntry eventEntry)
        {
            try
            {
                await _soundPlayerService.ProcessEvent(eventEntry);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error add event to sound player:\n{ex}");
            }
        }
        #endregion


        async Task SaveEvents()
        {
            await ExportCurrentEvents(true);
        }
        async Task AckEvents()
        {
            await ExportCurrentEvents(false);
        }
        private void Start()
        {
            _eventsProvider.Start();
        }
        private void Stop()
        {
            _eventsProvider.Stop();
        }

        public AsyncCommand SaveCommand { get; private set; }
        public AsyncCommand AckCommand { get; private set; }
        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand ClearSoundQueueCommand { get; private set; }

        private void SetCommands()
        {
            SaveCommand = new AsyncCommand(SaveEvents);
            AckCommand = new AsyncCommand(AckEvents);

            StartCommand = new DelegateCommand(
                () =>
                {
                    Start();
                },
                () => !SubscribeStatus);

            StopCommand = new DelegateCommand(
                () =>
                {
                    Stop();
                },
                () => SubscribeStatus);

            ClearSoundQueueCommand = new DelegateCommand(
                () =>
                {
                    _soundPlayerService.AcknowledgeEvents();
                });
        }
    }
}
