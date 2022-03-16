using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNV.LogWatcher.DataTransfer
{
    public class DiagInfoRecievedEventArgs : EventArgs
    {
        public DiagnosticsInformation DiagInfo { get; }
        public DiagInfoRecievedEventArgs(DiagnosticsInformation diagInfo)
        {
            DiagInfo = diagInfo;
        }
    }
}
