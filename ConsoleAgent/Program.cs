using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            var collector = new Collector();

            // 1. use format description
            collector.Collect(true);
            // 2. by props
            collector.Collect(false);

            Console.WriteLine($"Finished!");
            Console.ReadKey();
        }
    }

    class Collector
    {
        public void Collect(bool useFormatDescription)
        {
            string eventReaderQuery = "*[System[Level = 1 or Level = 2]/TimeCreated/@SystemTime >= '{0}']";
            string timeFormatString = "yyyy-MM-ddTHH:mm:ss.ffffff00OK";

            string timeSpanString = DateTime.Now.AddHours(-7).ToString(timeFormatString, CultureInfo.InvariantCulture);
            string query = string.Format(eventReaderQuery, timeSpanString);

            var eventRecords = new List<string>();
            var reflectedEventRecords = new List<string>();
            var exceptions = new List<string>();

            var watcher = Stopwatch.StartNew();
            foreach (var eventLogName in EventLog.GetEventLogs())
            {
                var eventsQuery = new EventLogQuery(eventLogName.Log, PathType.LogName, query) { ReverseDirection = true };
                var logReader = new EventLogReader(eventsQuery);

                for (EventRecord eventRecord = logReader.ReadEvent(); eventRecord != null; eventRecord = logReader.ReadEvent())
                {
                    // reflection
                    try
                    {
                        //reflectedEventRecords.Add("========\n");
                        //var propsInfoList = eventRecord.GetType().GetProperties();
                        //foreach (var pi in propsInfoList)
                        //{
                        //    var propName = pi.Name;
                        //    var propType = pi.PropertyType;
                        //    string propValue = "";
                        //    if (propType is IEnumerable)
                        //    {
                        //        if (pi.GetValue(eventRecord) != null)
                        //        {
                        //            propValue = string.Join(";", pi.GetValue(eventRecord));
                        //        }
                        //    }
                        //    else
                        //    {
                        //        if (pi.GetValue(eventRecord) != null)
                        //        {
                        //            propValue = pi.GetValue(eventRecord).ToString();
                        //        }
                        //    }
                        //    reflectedEventRecords.Add($"Name = {propName}, Type = {propType}, Value = {propValue}");
                        //}
                        //reflectedEventRecords.Add("========\n");
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add("Err refl" + ex.ToString());
                    }

                    try
                    {
                        eventRecords.Add(new EventEntry(eventRecord, useFormatDescription).ToString());
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add($"Error parsing event record {eventRecord.ProviderName}" + ex.ToString());
                    }
                }
            }
            watcher.Stop();
            Console.WriteLine($"Elapsed collecting time (useFormatDescription = {useFormatDescription}): {watcher.Elapsed}");

            string fileName = $"EventRecords_{useFormatDescription}.txt";
            File.WriteAllLines(fileName, eventRecords);
            FileInfo fi = new FileInfo(fileName);
            Console.WriteLine($"Records file size = {Math.Round((double)fi.Length / 1024 / 1024, 6)} MB");

            Console.WriteLine($"Exceptions thrown: {exceptions.Count}");
            File.WriteAllLines("exc.txt", exceptions);
        }
    }
}
