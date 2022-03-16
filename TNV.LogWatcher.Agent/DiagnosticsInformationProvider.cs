using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using TNV.LogWatcher.DataTransfer;

namespace TNV.LogWatcher.Agent
{
    class DiagnosticsInformationProvider : IDiagnosticsInformationProvider
    {
        private readonly List<string> _logNames = new List<string>();

        public DiagnosticsInformation GetDiagnosticsInformation()
        {
            var diagInfo = new DiagnosticsInformation()
            {
                User = GetUserIdentity(),
                UserDomain = GetUserDomain(),
                IpAddress = GetIpAddress(),
                MachineName = GetHostName(),
                OSVersion = GetOSVersion(),
                DomainName = GetDomainName(),
                LogNames = GetEventLogNames()
            };

            return diagInfo;
        }

        private string GetDomainName()
        {
            string domainName;
            try
            {
                domainName = System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain().Name;
            }
            catch
            {
                domainName = "";
            }
            return domainName;
        }

        private string GetHostName()
        {
            return Environment.MachineName;
        }

        private string GetOSVersion()
        {
            return Environment.OSVersion.ToString();
        }

        private string GetUserIdentity()
        {
            return $"{Environment.UserName}";
        }

        private string GetUserDomain()
        {
            return $"{Environment.UserDomainName}";
        }

        private string GetIpAddress()
        {
            string ipAddress = "";
            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (var ip in networkInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            ipAddress = ip.Address.ToString();
                        }
                    }
                }
            }

            return ipAddress;
        }

        private List<string> GetEventLogNames()
        {
            return _logNames;
        }

        public void AddMonitoredLogName(string logName)
        {
            _logNames.Add(logName);
        }

    }
}
