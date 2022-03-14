using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    interface ITimerService
    {
        event EventHandler EventsDisplayPeriodElapsed;
        event EventHandler LiveSignalTimeoutElapsed;

        void Start();
        void Stop();
    }
}
