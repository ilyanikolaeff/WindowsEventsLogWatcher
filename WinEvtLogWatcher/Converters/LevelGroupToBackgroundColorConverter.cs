using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WinEvtLogWatcher
{
    class LevelGroupToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var settings = Settings.GetInstance();
            var stdLevel = (StandardEventLevel)value;
            var color = settings.ColorSettings.FirstOrDefault(p => p.LevelName == stdLevel.ToString()).BackgroundColor;
            if (color == null)
                return null;
            return new SolidColorBrush(MDColorConverter.ToMediaColor(color));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
