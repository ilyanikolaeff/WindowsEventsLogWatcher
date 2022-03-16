using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TNV.LogWatcher.Manager
{
    class TimerService : ITimerService
    {
        public event EventHandler EventsDisplayPeriodElapsed;
        public event EventHandler LiveSignalTimeoutElapsed;

        private readonly Settings _settings;
        private readonly Timer _eventsDisplayTimer;
        private readonly Timer _liveSignalTimer;

        private int _lasthour = -1;
        public TimerService(Settings settings)
        {
            _settings = settings;
            _settings.SettingsChanged += OnSettingsChanged;

            _eventsDisplayTimer = new Timer(_settings.EventsDisplayPeriod > 0 ? _settings.EventsDisplayPeriod * 60000 : int.MaxValue)
            {
                Enabled = false,
                AutoReset = true
            };
            _eventsDisplayTimer.Elapsed += OnInternalTimerElapsed;

            _liveSignalTimer = new Timer(1000)
            {
                Enabled = true,
                AutoReset = true
            };
            _liveSignalTimer.Elapsed += OnLiveSignalTimerElapsed;
        }

        private void OnLiveSignalTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var currentTime = DateTime.Now;
            if (_lasthour != currentTime.Hour && currentTime.Minute == 0)
            {
                _lasthour = currentTime.Hour;
                LiveSignalTimeoutElapsed?.Invoke(this, new EventArgs());
            }
        }

        private void OnInternalTimerElapsed(object sender, ElapsedEventArgs e)
        {
            EventsDisplayPeriodElapsed?.Invoke(this, new EventArgs());
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            SetInterval();
        }

        public void Start()
        {
            // первоначальный запуск
            if (_eventsDisplayTimer.Interval != int.MaxValue)
                _eventsDisplayTimer.Start();
        }

        public void Stop()
        {
            _eventsDisplayTimer.Stop();
        }

        private void SetInterval()
        {
            _eventsDisplayTimer.Interval = _settings.EventsDisplayPeriod > 0 ? _settings.EventsDisplayPeriod * 60000 : int.MaxValue;
            if (_settings.EventsDisplayPeriod == 0)
            {
                Stop();
            }
            else
            {
                Start();
            }
        }
    }
}
