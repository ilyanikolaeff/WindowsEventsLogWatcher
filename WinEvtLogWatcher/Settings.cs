using DevExpress.Xpf.Core;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace WinEvtLogWatcher
{
    public class Settings
    {
        private static Settings _instance;
        public static Settings GetInstance()
        {
            if (_instance == null)
                _instance = Deserialize();

            return _instance;
        }
        private Settings()
        { }

        // settings
        public string QueryString { get; set; } = "*[System[Level=1 or Level=2 or Level=3]]";
        public int MaxEventsToDisplay { get; set; } = 1000;
        public bool AutoSubscribeOnStartUp { get; set; } = false;

        private int _eventsDisplayPeriod;
        public int EventsDisplayPeriod 
        {
            get => _eventsDisplayPeriod;
            set
            {
                _eventsDisplayPeriod = value;
                OnSettingsChanged();
            }
        }

        public int DiagnosePeriod { get; set; } = 60;
        public int DelayAfterBadDiagnose { get; set; } = 180; // 3 min
        public int TryRecreateLogWatcherCount { get; set; } = 3;

        public bool EnableMonitoringSound { get; set; } = true;

        private bool _isAlwaysOnTop;
        public bool IsAlwaysOnTop
        {
            get => _isAlwaysOnTop;
            set
            {
                _isAlwaysOnTop = value;
                OnSettingsChanged();
            }
        }
        public bool UseDarkTheme { get; set; } = false;
        public List<SoundSetting> SoundSettings { get; set; }

        public List<ColorSetting> ColorSettings { get; set; }
        public List<HostSetting> HostSettings { get; set; }



        public event EventHandler SettingsChanged;
        private void OnSettingsChanged()
        {
            SettingsChanged?.Invoke(this, new EventArgs());
        }

        private static Settings Deserialize()
        {
            var fileName = "Settings.xml";
            var xmlSerializer = new XmlSerializer(typeof(Settings));
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                var settings = (Settings)xmlSerializer.Deserialize(fs);
                return settings;
            }
        }

        public static void Serialize(Settings settings)
        {
            var fileName = "Settings.xml";
            var xmlSerializer = new XmlSerializer(typeof(Settings));
            using (var fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                xmlSerializer.Serialize(fs, settings);
            }
        }

        private void SetDefaultValues()
        {
            //HostSettings = new List<HostSetting>();

            //SoundSettings = new List<SoundSetting>()
            //{
            //    new SoundSetting() { LevelName = "Critical", FileName = @"Sounds\Critical.wav", IsCycle = true },
            //    new SoundSetting() { LevelName = "Error", FileName = @"Sounds\Error.wav", IsCycle = true },
            //    new SoundSetting() { LevelName = "Warning", FileName = @"Sounds\Warning.wav", IsCycle = false },
            //    new SoundSetting() { LevelName = "Informational", FileName = @"Sounds\Informational.wav", IsCycle = false },
            //    new SoundSetting() { LevelName = "Verbose", FileName = @"Sounds\Verbose.wav", IsCycle = false },
            //};

            //ColorSettings = new List<ColorSetting>()
            //{
            //    new ColorSetting() { LevelName = "Critical", FontColor = Color.FromArgb(255, 255, 0), BackgroundColor = Color.FromArgb(255, 0, 0) },
            //    new ColorSetting() { LevelName = "Error", FontColor = Color.FromArgb(0, 0, 0), BackgroundColor = Color.FromArgb(255, 0, 0) },
            //    new ColorSetting() { LevelName = "Warning", FontColor = Color.FromArgb(0, 0, 0), BackgroundColor = Color.FromArgb(255, 255, 0) },
            //    new ColorSetting() { LevelName = "Informational", FontColor = Color.FromArgb(0, 0, 0), BackgroundColor = Color.FromArgb(255, 255, 255) },
            //    new ColorSetting() { LevelName = "Verbose", FontColor = Color.FromArgb(0, 0, 0), BackgroundColor = Color.FromArgb(255, 255, 255) },
            //};

        }
    }

    public class SoundSetting
    {
        [XmlAttribute]
        public string LevelName { get; set; }
        [XmlAttribute]
        public string FileName { get; set; }
        [XmlAttribute]
        public bool IsCycle { get; set; }
    }

    public class HostSetting
    {
        [XmlAttribute]
        public string IpAddress { get; set; }
        
        [XmlAttribute]
        public string Domain { get; set; }

        [XmlAttribute]
        public string User { get; set; }

        [XmlAttribute]
        public string Password { get; set; }

        public List<string> ExcludingJournalNames { get; set; }

        public HostSetting()
        {
            ExcludingJournalNames = new List<string>();
        }
    }
}
