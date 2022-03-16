using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core.ConditionalFormatting;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TNV.LogWatcher.DataTransfer;

namespace TNV.LogWatcher.Manager.ViewModels
{
    class EventsGridViewModel : ViewModelBase
    {
        #region Fields
        private readonly ITransferService _transferService;
        private readonly ITimerService _timerService;
        private readonly Settings _settings;
        private readonly ISoundPlayerService _soundPlayerService;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IHistoryService _historyService;
        #endregion

        #region Properties

        public DiagnosticsInformationViewModel DiagnosticsViewModel
        {
            get => GetValue<DiagnosticsInformationViewModel>();
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

        public string ApplicationState
        {
            get => $"Состояние работы - {SubscribeStatus}, адрес биндинга - net.tcp://{_settings.ServiceAddress}/{_settings.ServiceName}";
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

        public ObservableCollection<FormattingRule> Rules { get; set; }

        #endregion

        #region Constructors
        public EventsGridViewModel(ITransferService transferService, ITimerService timerService, ISoundPlayerService soundPlayerService, 
            IHistoryService historyService, Settings settings)
        {
            _transferService = transferService ?? throw new ArgumentNullException("eventsProvider");
            _timerService = timerService ?? throw new ArgumentNullException("timerService");
            _soundPlayerService = soundPlayerService ?? throw new ArgumentNullException("soundPlayerService");
            _historyService = historyService ?? throw new ArgumentNullException("historyService");
            _settings = settings;

            _transferService.EventsReceived += OnNewEventsReceived;
            _timerService.EventsDisplayPeriodElapsed += OnTimerElapsed;
            _timerService.LiveSignalTimeoutElapsed += OnLiveSignalTimerElapsed;


            SetCommands();
            ApplyFormatRules();
        }

        private void ApplyFormatRules()
        {
            Rules = new ObservableCollection<FormattingRule>()
            {
                new FormattingRule("Level", ConditionRule.Equal, 1, true, GetFormat("Critical")),
                new FormattingRule("Level", ConditionRule.Equal, 2, true, GetFormat("Error")),
                new FormattingRule("Level", ConditionRule.Equal, 3, true, GetFormat("Warning")),
                new FormattingRule("Level", ConditionRule.Equal, 4, true, GetFormat("Informational")),
                new FormattingRule("Level", ConditionRule.Equal, 5, true, GetFormat("Critical")),
            };
        }

        private async void OnNewEventsReceived(object sender, EventsRecievedEventArgs e)
        {
            var taskList = new List<Task>();
            foreach (var eventEntry in e.EventEntries)
                taskList.Add(ProcessNewEventEntry(eventEntry));
            await Task.WhenAll(taskList);
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
                        if (Events.Count > _settings.MaxEventsToDisplay)
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

        private void SetCommands()
        {
            SaveCommand = new AsyncCommand(SaveEvents);
            AckCommand = new DelegateCommand(AckEvents);
            ClearSoundQueueCommand = new DelegateCommand(ClearSoundQueue);
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

        private Format GetFormat(string levelName)
        {
            var colorSettings = _settings.ColorSettings.FirstOrDefault(p => p.LevelName == levelName);
            if (colorSettings != null)
            {
                return new Format() { Background = colorSettings.Background, Foreground = colorSettings.Foreground };
            }
            return null;
        }
    }
}
