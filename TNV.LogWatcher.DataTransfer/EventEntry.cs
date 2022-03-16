using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNV.LogWatcher.DataTransfer
{
    public class EventEntry
    {
        public DateTime? TimeCreated { get; set; }
        public string MachineName { get; set; }
        public string ProviderName { get; set; }
        public string LogName { get; set; }
        public string Description { get; set; }
        public byte? Level { get; set; }
        public StandardEventLevel LevelGroup { get; set; }

        public EventEntry(EventRecord eventRecord)
        {
            if (eventRecord == null)
                throw new NullReferenceException("eventRecord");

            TimeCreated = eventRecord.TimeCreated;
            MachineName = eventRecord.MachineName;
            ProviderName = eventRecord.ProviderName;
            LogName = eventRecord.LogName;

            try
            {
                // Format description can throw error
                Description = eventRecord.FormatDescription();
            }
            catch
            {
                
            }

            // format description is empty, collecting by EventProperties
            if (string.IsNullOrEmpty(Description))
            {
                Description = string.Join("\n", eventRecord.Properties.Select(s => s.Value));
            }
            Level = eventRecord.Level;
            LevelGroup = (StandardEventLevel)eventRecord.Level;
        }

        public EventEntry()
        {

        }

        public override string ToString()
        {
            return $"{TimeCreated}\t{MachineName}\t{ProviderName}\t{LogName}\t{Description.Replace(Environment.NewLine, " ")}\t{Level}\t{LevelGroup}";
        }
    }
}
