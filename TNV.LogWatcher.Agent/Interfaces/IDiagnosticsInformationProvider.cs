using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNV.LogWatcher.DataTransfer;

namespace TNV.LogWatcher.Agent
{
    interface IDiagnosticsInformationProvider
    {
        DiagnosticsInformation GetDiagnosticsInformation();

        void AddMonitoredLogName(string logName);
    }
}
