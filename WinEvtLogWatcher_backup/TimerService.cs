using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WinEvtLogWatcher
{
    class TimerService : ITimerService
    {
        public event EventHandler TimerElapsed;

        private readonly Settings _settings;
        private readonly Timer _internalTimer = new Timer() { Enabled = false };
        public TimerService(Settings settings)
        {
            _settings = settings ?? throw new ArgumentNullException("settings");
            _settings.SettingsChanged += OnSettingsChanged;
            _internalTimer.Elapsed += OnTimerElapsed;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            TimerElapsed?.Invoke(this, new EventArgs());
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            if (_internalTimer.Interval != _settings.EventsDisplayPeriod * 60000)
                _internalTimer.Interval = _settings.EventsDisplayPeriod * 60000;
        }

        public void Start()
        {
            if (_settings.EventsDisplayPeriod > 0)
            {
                _internalTimer.Interval = _settings.EventsDisplayPeriod * 60000;
                _internalTimer.Enabled = true;
            }
        }

        public void Stop()
        {
            _internalTimer.Enabled = false;
        }
    }
}
