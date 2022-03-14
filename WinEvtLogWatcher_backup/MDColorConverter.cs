using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MColor = System.Windows.Media.Color;
using DColor = System.Drawing.Color;

namespace WinEvtLogWatcher
{
    class MDColorConverter
    {
        public static MColor ToMediaColor(DColor color)
        {
            return MColor.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
