using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNV.LogWatcher.Manager
{
    interface ITimerService
    {
        event EventHandler EventsDisplayPeriodElapsed;
        event EventHandler LiveSignalTimeoutElapsed;

        void Start();
        void Stop();
    }
}
