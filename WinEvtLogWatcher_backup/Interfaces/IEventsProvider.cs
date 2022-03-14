using System;

namespace WinEvtLogWatcher
{
    interface IEventsProvider
    {
        void Start();
        void Stop();

        event EventHandler<NewEventEntryComeEventArgs> NewEventEntryAppeared;
    }
}
