using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Core;
using NLog;

namespace WinEvtLogWatcher
{
    /// <summary>
    /// Interaction logic for HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public HistoryWindow()
        {
            InitializeComponent();
            //DataContext = this;
            //CurrentLoadedEvents = new ObservableCollection<WatcherEventRecord>();

            //// Получаем имена всех файлов в директории с историей
            //GetFilesNames();
            //// Инициируем начальную загрузку
            //LoadEventsAsync();
        }
        //public ObservableCollection<WatcherEventRecord> CurrentLoadedEvents { get; set; }

        //private int _loadFilesCount = 10;

        //private object _locker = new object();

        //private List<string> FilesNames = new List<string>();
        //private void GetFilesNames()
        //{
        //    try
        //    {
        //        var directoryPath = AppDomain.CurrentDomain.BaseDirectory + @"\History\";
        //        var files = Directory.GetFiles(directoryPath, "*.txt").ToList().OrderByDescending(ks => new FileInfo(ks).CreationTime);
        //        FilesNames = files.ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error("История не найдена. Записи не будут загружены");
        //    }
        //}

        //private async void LoadEventsAsync()
        //{
        //    try
        //    {
        //        gridControl.BeginDataUpdate();
        //        await Task.Run(() =>
        //        {
        //            List<string> filesNames = new List<string>();
        //            if (FilesNames.Count > _loadFilesCount)
        //            {
        //                filesNames.AddRange(FilesNames.Take(_loadFilesCount));
        //                FilesNames.RemoveRange(0, _loadFilesCount);
        //            }
        //            else
        //            {
        //                filesNames.AddRange(FilesNames);
        //                FilesNames.Clear();
        //            }

        //            foreach (var file in filesNames)
        //            {
        //                LoadEventsFromFile(file);
        //            }
        //        });
        //        gridControl.EndDataUpdate();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error($"Ошибка асинхронной обработки загрузки событий: {ex}");
        //    }
        //}

        //async void LoadEventsFromFile(string fileName)
        //{
        //    await Task.Run(() =>
        //    {
        //        var readedEvents = File.ReadAllLines(fileName);
        //        int i = 1;
        //        foreach (var eventRecord in readedEvents.Skip(1))
        //        {
        //            try
        //            {
        //                if (!string.IsNullOrEmpty(eventRecord))
        //                {
        //                    lock (_locker)
        //                    {
        //                        if (Application.Current != null)
        //                        {
        //                            Application.Current.Dispatcher.Invoke(() =>
        //                            {
        //                                CurrentLoadedEvents.Add(new WatcherEventRecord(eventRecord));
        //                            });
        //                        }
        //                    }
        //                }
        //                i++;
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.Error($"Ошибка асинхронного чтения событий. Имя файла: {fileName}, " +
        //                    $"запись:  {eventRecord}, номер строки: {i}. Описание ошибки: {ex}");
        //            }
        //        }
        //    });
        //}

        //private void eventsTableView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        //{
        //    if (e.VerticalChange > 0)
        //    {
        //        if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight)
        //        {
        //            LoadEventsAsync();
        //        }
        //    }
        //}
    }
}
