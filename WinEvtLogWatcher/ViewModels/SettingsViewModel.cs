using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WinEvtLogWatcher.ViewModels
{
    class SettingsViewModel : ViewModelBase
    {
        #region Properties

        public bool IsAlwaysOnTop
        {
            get => _settingsProvider.IsAlwaysOnTop;
            set
            {
                _settingsProvider.IsAlwaysOnTop = value;
                RaisePropertiesChanged();
            }
        }

        public int MaxEventsToDisplay
        {
            get => _settingsProvider.MaxEventsToDisplay;
            set { _settingsProvider.MaxEventsToDisplay = value; RaisePropertyChanged(); }
        }
        public int EventsDisplayPeriod
        {
            get => _settingsProvider.EventsDisplayPeriod;
            set 
            { 
                _settingsProvider.EventsDisplayPeriod = value; 
                RaisePropertyChanged(); 
            }
        }
        public bool UseDarkTheme
        {
            get => _settingsProvider.UseDarkTheme;
            set
            {
                if (!value)
                    ApplicationThemeHelper.ApplicationThemeName = Theme.Office2019ColorfulName;
                else
                    ApplicationThemeHelper.ApplicationThemeName = Theme.VS2017DarkName;

                _settingsProvider.UseDarkTheme = value;
            }
        }

        #region Sounds files
        public string CriticalSoundFile
        {
            get { return GetSoundFile("Critical"); }
            set { SetSoundFile("Critical", value); }
        }
        public string ErrorSoundFile
        {
            get { return GetSoundFile("Error"); }
            set { SetSoundFile("Error", value); }
        }
        public string WarningSoundFile
        {
            get { return GetSoundFile("Warning"); }
            set { SetSoundFile("Warning", value); }
        }
        public string InfoSoundFile
        {
            get { return GetSoundFile("Informational"); }
            set { SetSoundFile("Informational", value); }
        }
        public string VerboseSoundFile
        {
            get { return GetSoundFile("Verbose"); }
            set { SetSoundFile("Verbose", value); }
        }
        #endregion

        #endregion

        #region Fields
        readonly IDialogService dialogService = new DefaultDialogService();
        private readonly Settings _settingsProvider = Settings.GetInstance();
        #endregion

        #region Contructors
        public SettingsViewModel()
        {
            SetCommands();
        }
        #endregion

        #region Commands
        public ICommand TestPlaySoundCommand { get; private set; }
        public ICommand OpenSoundFileCommand { get; private set; }
        #endregion

        private void SetSoundFile(string eventLevel, string fileName)
        {
            var soundSetting = _settingsProvider.SoundSettings.FirstOrDefault(p => p.LevelName == eventLevel);
            if (soundSetting != null)
                soundSetting.FileName = fileName;
            RaisePropertiesChanged();
        }

        private string GetSoundFile(string eventLevel)
        {
            var soundSetting = _settingsProvider.SoundSettings.FirstOrDefault(p => p.LevelName == eventLevel);
            return soundSetting == null ? "" : soundSetting.FileName;
        }

        private void SetCommands()
        {
            TestPlaySoundCommand = new DelegateCommand<string>(
                (fileName) =>
                {
                    var soundPlayer = new SoundPlayer(fileName);
                    soundPlayer.Play();
                },
                (fileName) =>
                {
                    return File.Exists(fileName);
                }
                );

            OpenSoundFileCommand = new DelegateCommand<object>(
                (level) =>
                {
                    if (dialogService.OpenFileDialog() == true)
                    {
                        var intValue = Convert.ToInt32(level);
                        var fileName = dialogService.FilePath;
                        if (intValue == 1)
                            CriticalSoundFile = fileName;
                        else if (intValue == 2)
                            ErrorSoundFile = fileName;
                        else if (intValue == 3)
                            WarningSoundFile = fileName;
                        else if (intValue == 4)
                            InfoSoundFile = fileName;
                        else if (intValue == 5)
                            VerboseSoundFile = fileName;
                    }
                }
                );
        }
    }
}
