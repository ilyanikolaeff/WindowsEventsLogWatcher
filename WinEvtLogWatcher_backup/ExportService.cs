using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    class ExportService : IExportService
    {
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

                    string separator = fileName.EndsWith(".csv") ? ";" : "\t";
                    if (events != null)
                    {
                        foreach (var evt in events)
                        {
                            if (evt != null)
                            {
                                // если хоть одно из нужных свойств 
                                if (evt.TimeCreated == null || evt.Level == null)
                                    continue;

                                var text = $"{evt.TimeCreated.Value:dd.MM.yyyy HH:mm:ss}{separator}" +
                                    $"{evt.MachineName}{separator}" +
                                    $"{evt.ProviderName}{separator}" +
                                    $"{evt.LogName}{separator}" +
                                    // Короче тут заплатка из-за того, что описание события может быть из нескольких строк, а открывать в Excel и Notepad хочется нормально
                                    $"{string.Join(" ", evt.Description.Split('\n'))}{separator}" + 
                                    $"{evt.Level}{separator}" +
                                    $"{(StandardEventLevel)evt.Level}";

                                exportedFile.WriteLine(text);
                            }
                        }
                    }
                }
            }
        }

        public async Task ExportAlarmsToHistory(IEnumerable<EventEntry> events)
        {
            await Task.Run(() =>
            {
                string pathName = AppDomain.CurrentDomain.BaseDirectory + @"\History\";
                if (!Directory.Exists(pathName))
                    Directory.CreateDirectory(pathName);

                string fileName = pathName + $"{DateTime.Now:yyyy_MM_dd HH_mm_ss} History.txt";
                Export(events, fileName);
            });
        }

        public enum ClearType
        {
            Time,
            Count
        }
    }
}
