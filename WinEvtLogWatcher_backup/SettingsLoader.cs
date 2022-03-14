using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WinEvtLogWatcher
{
    class SettingsLoader
    {
        public Settings LoadSettings()
        {
            return Deserialize();
        }

        public void SaveSettings(Settings settings)
        {
            Serialize(settings);
        }

        private Settings Deserialize()
        {
            var fileName = "Settings.xml";
            var xmlSerializer = new XmlSerializer(typeof(Settings));
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                var settings = (Settings)xmlSerializer.Deserialize(fs);
                return settings;
            }
        }

        private void Serialize(Settings settings)
        {
            var fileName = "Settings.xml";
            var xmlSerializer = new XmlSerializer(typeof(Settings));
            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                xmlSerializer.Serialize(fs, settings);
            }
        }
    }
}
