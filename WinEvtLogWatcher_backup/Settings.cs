using DevExpress.Xpf.Core;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Drawing;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace WinEvtLogWatcher
{
    public class Settings
    {
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
        public bool UseDarkTheme { get; set; } = false;
        public List<SoundSetting> SoundSettings { get; set; } = new List<SoundSetting>()
        {
            new SoundSetting() { LevelName = "Critical", FileName = @"Sounds\Critical.wav", IsCycle = true },
            new SoundSetting() { LevelName = "Error", FileName = @"Sounds\Error.wav", IsCycle = true },
            new SoundSetting() { LevelName = "Warning", FileName = @"Sounds\Warning.wav", IsCycle = true },
            new SoundSetting() { LevelName = "Informational", FileName = @"Sounds\Informational.wav", IsCycle = true },
            new SoundSetting() { LevelName = "Verbose", FileName = @"Sounds\Verbose.wav", IsCycle = true },
        };

        public List<ColorSetting> ColorSettings { get; set; } = new List<ColorSetting>()
        {
            new ColorSetting() { LevelName = "Critical", FontColor = Color.FromArgb(255, 255, 0), BackgroundColor = Color.FromArgb(255, 0, 0) },
            new ColorSetting() { LevelName = "Error", FontColor = Color.FromArgb(0, 0, 0), BackgroundColor = Color.FromArgb(255, 0, 0) },
            new ColorSetting() { LevelName = "Warning", FontColor = Color.FromArgb(0, 0, 0), BackgroundColor = Color.FromArgb(255, 255, 0) },
            new ColorSetting() { LevelName = "Informational", FontColor = Color.FromArgb(0, 0, 0), BackgroundColor = Color.FromArgb(255, 255, 255) },
            new ColorSetting() { LevelName = "Verbose", FontColor = Color.FromArgb(0, 0, 0), BackgroundColor = Color.FromArgb(255, 255, 255) },
        };

        public List<HostSetting> HostSettings { get; set; } = new List<HostSetting>();



        public event EventHandler SettingsChanged;
        private void OnSettingsChanged()
        {
            SettingsChanged?.Invoke(this, new EventArgs());
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

    public class ColorSetting
    {
        [XmlAttribute]
        public string LevelName { get; set; }
        [XmlIgnore]
        public Color BackgroundColor { get; set; }

        [XmlAttribute("BackgroundColor")]
        public string BackgroundColorAsString
        {
            get => BackgroundColor.ToString();
            set => BackgroundColor = Color.FromName(value);
        }

        [XmlIgnore]
        public Color FontColor { get; set; }
        [XmlAttribute("FontColor")]
        public string FontColorAsString
        {
            get => FontColor.ToString();
            set => FontColor = Color.FromName(value);
        }
    }

    public class HostSetting
    {
        [XmlAttribute]
        public string IpAddress { get; set; }
        public List<string> ExcludingJournalNames { get; set; }

        public HostSetting()
        {
            ExcludingJournalNames = new List<string>();
        }
    }
}
