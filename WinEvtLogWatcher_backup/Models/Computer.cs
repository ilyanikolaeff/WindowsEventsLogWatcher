using NLog;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    class Computer
    {
        public string IpAddress { get; private set; }
        public string MachineName { get => GetMachineName(); }
        public EventLog[] EventsLogs { get => GetAvailableEventsLogs(); }

        public Computer(string ipAddress)
        {
            IpAddress = ipAddress;
        }
        private EventLog[] GetAvailableEventsLogs()
        {
            EventLog[] eventLogs = EventLog.GetEventLogs(IpAddress);
            return eventLogs;
        }

        private string GetMachineName()
        {
            var hostEntry = Dns.GetHostEntry(IpAddress);
            string machineName = hostEntry.HostName;
            return machineName;
        }
    }
}
