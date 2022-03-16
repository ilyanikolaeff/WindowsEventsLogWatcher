using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNV.LogWatcher.Agent
{
    interface ILogMonitorService
    {
        event EventHandler<NewEventEntryCome> NewEventEntryCome;
    }
}
