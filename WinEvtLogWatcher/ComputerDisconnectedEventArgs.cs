using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    class ComputerDisconnectedEventArgs : EventArgs
    {
        public readonly IEnumerable<Computer> DisconnectedComputers;
        public ComputerDisconnectedEventArgs(IEnumerable<Computer> disconnectedComputers)
        {
            DisconnectedComputers = disconnectedComputers;
        }
    }
}
