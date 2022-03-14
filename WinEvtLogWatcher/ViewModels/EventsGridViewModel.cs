using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private readonly ITimerService _timerService;
        private readonly Settings _settingsProvider = Settings.GetInstance();
        private readonly ISoundPlayerService _soundPlayerService;
        private readonly IApplicationLogger _logger;
        private readonly IHistoryService _historyService;
        #endregion

        #region Properties

        public LogSubscribersViewModel LogSubscribersViewModel
        {
            get => GetValue<LogSubscribersViewModel>();
            set => SetValue(value);
        }

        public SettingsViewModel SettingsViewModel
        {
            get => GetValue<SettingsViewModel>();
            set => SetValue(value);
        }

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

        public string FilterString
        {
            get => GetValue<string>();
            set
            {
                OnFilterStringChanged(FilterString, value);
                SetValue(value);
            }
        }
        #endregion

        #region Constructors
        public EventsGridViewModel(IEventsProvider eventsProvider, ITimerService timerService,
            ISoundPlayerService soundPlayerService, IApplicationLogger logger, IHistoryService historyService)
        {
            _eventsProvider = eventsProvider ?? throw new ArgumentNullException("eventsProvider");
            _timerService = timerService ?? throw new ArgumentNullException("timerService");
            _soundPlayerService = soundPlayerService ?? throw new ArgumentNullException("soundPlayerService");
            _logger = logger ?? throw new ArgumentNullException("logger");
            _historyService = historyService ?? throw new ArgumentNullException("historyService");

            _eventsProvider.NewEventEntryAppeared += OnNewEventEntryCome;
            _timerService.EventsDisplayPeriodElapsed += OnTimerElapsed;
            _timerService.LiveSignalTimeoutElapsed += OnLiveSignalTimerElapsed;

            SetCommands();
        }

        private void OnLiveSignalTimerElapsed(object sender, EventArgs e)
        {
            _soundPlayerService.TryPlayLiveSignal();
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
                        await _historyService.ExportAlarmsToHistory(currentEvents);
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
        private async void OnNewEventEntryCome(object sender, NewEventEntryComeEventArgs e)
        {
            if (e.NewEventEntry != null)
            {
                await ProcessNewEventEntry(e.NewEventEntry);
            }
        }


        private async Task ProcessNewEventEntry(EventEntry eventEntry)
        {
            var taskList = new List<Task>();
            taskList.Add(Task.Run(() => _historyService.SaveEntry(eventEntry)));
            //Task.Run(() => _historyService.SaveEntry(eventEntry));

            bool needProcess = true;
            if (!string.IsNullOrEmpty(FilterString))
            {
                CriteriaOperator op = CriteriaOperator.Parse(FilterString);
                var propDescrCollection = TypeDescriptor.GetProperties(typeof(EventEntry));
                ExpressionEvaluator evaluator = new ExpressionEvaluator(propDescrCollection, op, false);
                object evaluatorResult = evaluator.Evaluate(eventEntry);
                needProcess = Convert.ToBoolean(evaluatorResult);
            }

            if (needProcess)
            {
                //Task.Run(() => AddEventToTableView(eventEntry)).ConfigureAwait(true);
                taskList.Add(Task.Run(() => AddEventToTableView(eventEntry)));
                taskList.Add(Task.Run(() => AddEventToSoundPlayer(eventEntry)));
            }

            // execute task
            await Task.WhenAll(taskList);
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
        private void AddEventToSoundPlayer(EventEntry eventEntry)
        {
            try
            {
                _soundPlayerService.AddEvent(eventEntry);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error add event to sound player:\n{ex}");
            }
        }
        async Task SaveEvents()
        {
            await ExportCurrentEvents(true);
        }
        private void AckEvents()
        {
            _soundPlayerService.AcknowledgeEvents();
        }
        private void ClearSoundQueue()
        {
            _soundPlayerService.ClearSoundQueue();
        }
        private void Start()
        {
            _eventsProvider.Start();
            SubscribeStatus = true;
        }
        private void Stop()
        {
            _eventsProvider.Stop();
            SubscribeStatus = false;
        }
        private void SetCommands()
        {
            SaveCommand = new AsyncCommand(SaveEvents);
            AckCommand = new DelegateCommand(AckEvents);
            ClearSoundQueueCommand = new DelegateCommand(ClearSoundQueue);

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

        }

        private void OnFilterStringChanged(string oldFilterString, string newFilterString)
        {
            _logger.Info($"Changed Filter!\noldFilterString = {oldFilterString};\nnewFilterString = {newFilterString}");
        }
        #endregion

        #region Commands
        public AsyncCommand SaveCommand { get; private set; }
        public ICommand AckCommand { get; private set; }
        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand ClearSoundQueueCommand { get; private set; }

        #endregion
    }
}
