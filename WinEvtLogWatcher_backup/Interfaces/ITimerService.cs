using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    interface ITimerService
    {
        event EventHandler TimerElapsed;

        void Start();
        void Stop();
    }
}
