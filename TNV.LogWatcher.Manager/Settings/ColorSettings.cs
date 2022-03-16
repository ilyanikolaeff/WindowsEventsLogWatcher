using System.Windows.Media;
using System.Xml.Serialization;

namespace TNV.LogWatcher.Manager
{
    public class ColorSettings
    {
        [XmlAttribute]
        public string LevelName { get; set; }

        [XmlIgnore]
        public Brush Background => new SolidColorBrush(BackgroundColor);

        [XmlIgnore]
        public Color BackgroundColor { get; set; }

        [XmlAttribute("Background")]
        public string BgColor 
        {
            get => BackgroundColor.ToString();
            set => BackgroundColor = (Color)ColorConverter.ConvertFromString(value);
        }


        [XmlIgnore]
        public Brush Foreground => new SolidColorBrush(ForegroundColor);

        [XmlIgnore]
        public Color ForegroundColor { get; set; }

        [XmlAttribute("Foreground")]
        public string FgColor
        {
            get => ForegroundColor.ToString();
            set => ForegroundColor = (Color)ColorConverter.ConvertFromString(value);
        }
    }
}
