using System;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Windows.Data;

namespace TNV.LogWatcher.Manager
{
    class EventLevelToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var byteValue = (byte)value;
            return (StandardEventLevel)byteValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
