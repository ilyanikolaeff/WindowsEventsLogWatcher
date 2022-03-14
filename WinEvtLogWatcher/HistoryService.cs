using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    class HistoryService : IHistoryService
    {
        private readonly IApplicationLogger _logger;
        private object _locker = new object();
        private readonly BlockingCollection<EventEntry> _buffer = new BlockingCollection<EventEntry>();

        public void SaveEntry(EventEntry eventEntry)
        {
            _buffer.Add(eventEntry);
        }

        private void SaveEntries()
        {
            try
            {
                foreach (var entry in _buffer.GetConsumingEnumerable())
                {
                    File.AppendAllText(GetFileName(), entry.ToString() + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error saving file to history:\n{ex}");
            }
        }

        private string GetFileName()
        {
            int count = 1;
            string fullPath = $@"{AppDomain.CurrentDomain.BaseDirectory}\History\{DateTime.Now:yyyy_MM_dd} NoFilter.txt";
            string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            string extension = Path.GetExtension(fullPath);
            string path = Path.GetDirectoryName(fullPath);
            string newFullPath = fullPath;

            while (File.Exists(newFullPath) && new FileInfo(newFullPath).Length > 10 * 1024 * 1024)
            {
                string tempFileName = $"{fileNameOnly}_{count++}";
                newFullPath = Path.Combine(path, tempFileName + extension);
            }

            return newFullPath;
        }

        public HistoryService(IApplicationLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException("logger");
            Task.Run(SaveEntries);
        }

        private void Export(IEnumerable<EventEntry> events, string fileName)
        {
            bool appendHeader = false;
            if (!File.Exists(fileName))
                appendHeader = true;

            using (FileStream fileStream = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                using (StreamWriter exportedFile = new StreamWriter(fileStream))
                {
                    if (appendHeader)
                        exportedFile.WriteLine("Время события\tИмя компьютера\tИсточник события\tЖурнал\tОписание события\tУровень события\tГруппа события");

                    if (events != null)
                    {
                        foreach (var evt in events)
                        {
                            exportedFile.WriteLine(evt.ToString());
                        }
                    }
                }
            }
        }

        public async Task ExportAlarmsToHistory(IEnumerable<EventEntry> events)
        {
            if (events.Count() > 0)
            {
                await Task.Run(() =>
                {
                    string pathName = AppDomain.CurrentDomain.BaseDirectory + @"\History\";
                    if (!Directory.Exists(pathName))
                        Directory.CreateDirectory(pathName);

                    string fileName = pathName + $"{DateTime.Now:yyyy_MM_dd HH_mm_ss} SavedHistory.txt";
                    Export(events, fileName);
                });
            }
        }

        public enum ClearType
        {
            Time,
            Count
        }
    }
}
