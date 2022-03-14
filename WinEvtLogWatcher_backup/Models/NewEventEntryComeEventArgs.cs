using System;

namespace WinEvtLogWatcher
{
    class NewEventEntryComeEventArgs : EventArgs
    {
        public EventEntry NewEventEntry { get; }
        public NewEventEntryComeEventArgs(EventEntry eventEntry)
        {
            NewEventEntry = eventEntry;
        }
    }

}
