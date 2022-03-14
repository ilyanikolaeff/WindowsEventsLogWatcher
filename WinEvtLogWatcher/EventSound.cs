namespace WinEvtLogWatcher
{
    class EventSound
    {
        public readonly string FileName;
        public readonly bool IsCycle;
        public readonly int Priority;

        public EventSound(string fileName, bool isCycle, int priority)
        {
            FileName = fileName;
            IsCycle = isCycle;
            Priority = priority;
        }
    }
}
