using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    class ComputersService : IComputersService
    {
        private readonly Settings _settingsProvider;

        public IEnumerable<Computer> GetComputers()
        {
            foreach (var host in _settingsProvider.HostSettings)
            {
                yield return new Computer(host.IpAddress);
            }
        }

        public ComputersService(Settings settingsProvider)
        {
            _settingsProvider = settingsProvider ?? throw new ArgumentNullException("settingsProvider");
        }
    }
}
