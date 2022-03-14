using NLog;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    class Computer
    {
        public string Domain { get; }
        public string User { get; }
        public string Password { get; }

        public string IpAddress { get; }
        public string MachineName { get; }
        public EventLog[] EventsLogs { get; }

        public Computer(string ipAddress, string domain, string user, string password, string machineName, EventLog[] eventLogs)
        {
            Domain = domain;
            User = user;
            Password = password;
            IpAddress = ipAddress;
            MachineName = machineName;
            EventsLogs = eventLogs;
        }

        public override string ToString()
        {
            return $"Computer: Ip = {IpAddress}";
        }
    }
}
