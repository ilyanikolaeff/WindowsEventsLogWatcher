using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAgent
{
    public class EventEntry
    {
        public DateTime? TimeCreated
        {
            get;
            private set;
        }
        public string MachineName
        {
            get;
            private set;
        }
        public string ProviderName
        {
            get;
            private set;
        }
        public string LogName
        {
            get;
            private set;
        }
        public string Description
        {
            get;
            private set;
        }
        public byte? Level
        {
            get;
            private set;
        }
        public StandardEventLevel LevelGroup
        {
            get;
            private set;
        }

        public EventEntry(EventRecord eventRecord, bool useFD)
        {
            if (eventRecord == null)
                throw new NullReferenceException("eventRecord");

            TimeCreated = eventRecord.TimeCreated;
            MachineName = eventRecord.MachineName;
            ProviderName = eventRecord.ProviderName;
            LogName = eventRecord.LogName;

            if (useFD)
                Description = eventRecord.FormatDescription();
            else
                Description = string.Join("\n", eventRecord.Properties.Select(s => s.Value));

            Level = eventRecord.Level;
            LevelGroup = (StandardEventLevel)eventRecord.Level;
        }

        public override string ToString()
        {
            return $"{TimeCreated}\t{MachineName}\t{ProviderName}\t{LogName}\t{Description.Replace(Environment.NewLine, " ")}\t{Level}\t{LevelGroup}";
        }
    }
}
