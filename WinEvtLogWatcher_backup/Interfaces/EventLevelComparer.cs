using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    class EventLevelComparer
    {
        private readonly Dictionary<int, string> _eventLevels = new Dictionary<int, string>()
        {
            { 1, "Critical" },
            { 2, "Error" },
            { 3, "Warning" },
            { 4, "Informational" },
            { 5, "Verbose" }
        };

        public string GetLevelName(int level)
        {
            return _eventLevels[level];
        }

        public int GetLevel(string levelName)
        {
            foreach (var eventLevel in _eventLevels)
            {
                if (eventLevel.Value.Equals(levelName, StringComparison.OrdinalIgnoreCase))
                    return eventLevel.Key;
            }
            return -1;
        }
    }
}
