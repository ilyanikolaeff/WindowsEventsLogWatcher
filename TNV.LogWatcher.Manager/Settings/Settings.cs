using DevExpress.Xpf.Core;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TNV.LogWatcher.Manager
{
    public class Settings
    {
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
        public List<SoundSettings> SoundSettings { get; set; }
        public List<ColorSettings> ColorSettings { get; set; }

        public string ServiceAddress { get; set; } = "127.0.0.1:65500";
        public string ServiceName { get; set; } = "LogWatcher";


        public event EventHandler SettingsChanged;
        private void OnSettingsChanged()
        {
            SettingsChanged?.Invoke(this, new EventArgs());
        }

        public static Settings Deserialize()
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
        }
    }

    public class SoundSettings
    {
        [XmlAttribute]
        public string LevelName { get; set; }
        [XmlAttribute]
        public string FileName { get; set; }
        [XmlAttribute]
        public bool IsCycle { get; set; }
    }
}
