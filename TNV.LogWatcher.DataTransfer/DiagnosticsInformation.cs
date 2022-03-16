using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNV.LogWatcher.DataTransfer
{
    public class DiagnosticsInformation
    {
        public string MachineName { get; set; }
        public string DomainName { get; set; }
        public string User { get; set; }
        public string UserDomain { get; set; }
        public string IpAddress { get; set; }
        public List<string> LogNames { get; set; }
        public string OSVersion { get; set; }
    }
}
