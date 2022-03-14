using System.Diagnostics.Eventing.Reader;

namespace WinEvtLogWatcher
{
    internal class EventLogWatcherExtended : EventLogWatcher
    {
        public Computer Computer { get; }
        public string LogName { get; }

        public bool IsInitialized { get; }
        public EventLogWatcherExtended(EventLogQuery eventLogQuery, string logName, Computer computer, bool isInitialized) : base(eventLogQuery)
        {
            LogName = logName;
            Computer = computer;
            IsInitialized = isInitialized;
        }

    }
}
