using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    interface IApplicationLogger
    {
        void Info(string message);
        void Debug(string message);
        void Trace(string message);
        void Error(string message);
        void Error(Exception ex);
    }
}
