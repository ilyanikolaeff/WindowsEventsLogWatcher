using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
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
            catch(Exception ex)
            {
                _logger.Error($"Error creating new event entry: \n{ex}");
            }

            // format description is empty, collecting by EventProperties
            if (string.IsNullOrEmpty(Description))
            {
                Description = string.Join("\n", eventRecord.Properties.Select(s => s.Value));
            }
            Level = eventRecord.Level;
            LevelGroup = (StandardEventLevel)eventRecord.Level;
        }

        public override string ToString()
        {
            return $"{TimeCreated}\t{MachineName}\t{ProviderName}\t{LogName}\t{Description.Replace(Environment.NewLine, " ")}\t{Level}\t{LevelGroup}";
        }

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    }
}
