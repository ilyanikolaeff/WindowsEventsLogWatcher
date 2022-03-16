using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNV.LogWatcher.Agent
{
    public class Configuration
    {
        public string ServiceAddress { get; set; } = "net.tcp://localhost:65500";
        public string ServiceName { get; set; } = "LogWatcher";
        public int EventCollectionTime { get; set; } = 1000;
        public int DiagInfoSendingInterval { get; set; } = 60000;
        public string QueryString { get; set; } = "*[System[Level=1 or Level=2 or Level=3 or Level=4]]";
        public int TryCreateCount { get; set; } = 3;
    }
}
