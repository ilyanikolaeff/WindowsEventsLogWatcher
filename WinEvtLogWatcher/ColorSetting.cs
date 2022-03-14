using System.Drawing;
using System.Xml.Serialization;

namespace WinEvtLogWatcher
{
    public class ColorSetting
    {
        [XmlAttribute]
        public string LevelName { get; set; }
        [XmlIgnore]
        public Color BackgroundColor 
        {
            get => Color.FromArgb(BackgroundColorAsArgb);
            set => BackgroundColorAsArgb = value.ToArgb(); 
        }

        [XmlAttribute("BackgroundColor")]
        public int BackgroundColorAsArgb
        {
            get;
            set;
        }

        [XmlIgnore]
        public Color FontColor 
        {
            get => Color.FromArgb(FontColorAsArgb);
            set => FontColorAsArgb = value.ToArgb();
        }

        [XmlAttribute("FontColor")]
        public int FontColorAsArgb
        {
            get;
            set;
        }
    }
}
