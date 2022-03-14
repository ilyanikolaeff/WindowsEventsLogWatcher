using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    //public class EventEntry
    //{
    //    private EventRecord _eventRecord;
    //    public DateTime? TimeCreated
    //    {
    //        get => _eventRecord.TimeCreated == null ? null : _eventRecord.TimeCreated;
    //    }
    //    public string MachineName
    //    {
    //        get => _eventRecord.MachineName;
    //    }
    //    public string ProviderName
    //    {
    //        get => _eventRecord.ProviderName;
    //    }
    //    public string LogName
    //    {
    //        get => _eventRecord.LogName;
    //    }
    //    public string Description
    //    {
    //        get => string.Join("\n", ((List<EventProperty>)_eventRecord.Properties).Select(s => s.Value));
    //    }
    //    public byte? Level
    //    {
    //        get => _eventRecord.Level == null ? null : _eventRecord.Level;
    //    }
    //    public StandardEventLevel LevelGroup
    //    {
    //        get => (StandardEventLevel)_eventRecord.Level;
    //    }

    //    public EventEntry(EventRecord eventRecord)
    //    {
    //        if (eventRecord == null)
    //            throw new NullReferenceException("eventRecord");
    //    }
    //}
}
