using DevExpress.Xpf.Core.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace TNV.LogWatcher.Manager.Converters
{
    public class LevelGroupToImageConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var levelGroup = value.ToString();
                if (levelGroup == "Critical") // critical
                    return WpfSvgRenderer.CreateImageSource(new Uri("pack://application:,,,/DevExpress.Images.v19.2;component/SvgImages/XAF/Action_Close.svg"));
                else if (levelGroup == "Error") // error
                    return WpfSvgRenderer.CreateImageSource(new Uri("pack://application:,,,/DevExpress.Images.v19.2;component/SvgImages/Icon Builder/Security_WarningCircled1.svg"));
                else if (levelGroup == "Warning") // warning
                    return WpfSvgRenderer.CreateImageSource(new Uri("pack://application:,,,/DevExpress.Images.v19.2;component/SvgImages/Status/Warning.svg"));
                else if (levelGroup == "Informational") // info
                    return WpfSvgRenderer.CreateImageSource(new Uri("pack://application:,,,/DevExpress.Images.v19.2;component/SvgImages/Icon Builder/Actions_Info.svg"));

                return null;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
