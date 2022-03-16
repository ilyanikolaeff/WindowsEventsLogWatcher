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

namespace TNV.LogWatcher.Manager.ViewModels
{
    class SettingsViewModel : ViewModelBase
    {
        #region Properties

        public bool IsAlwaysOnTop
        {
            get => _settings.IsAlwaysOnTop;
            set
            {
                _settings.IsAlwaysOnTop = value;
                RaisePropertiesChanged();
            }
        }

        public int MaxEventsToDisplay
        {
            get => _settings.MaxEventsToDisplay;
            set { _settings.MaxEventsToDisplay = value; RaisePropertyChanged(); }
        }
        public int EventsDisplayPeriod
        {
            get => _settings.EventsDisplayPeriod;
            set 
            { 
                _settings.EventsDisplayPeriod = value; 
                RaisePropertyChanged(); 
            }
        }
        public bool UseDarkTheme
        {
            get => _settings.UseDarkTheme;
            set
            {
                if (!value)
                    ApplicationThemeHelper.ApplicationThemeName = Theme.Office2019ColorfulName;
                else
                    ApplicationThemeHelper.ApplicationThemeName = Theme.VS2017DarkName;

                _settings.UseDarkTheme = value;
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
        readonly IDialogService _dialogService = new DefaultDialogService();
        private readonly Settings _settings;
        #endregion

        #region Contructors
        public SettingsViewModel(Settings settings)
        {
            _settings = settings;
            SetCommands();
        }
        #endregion

        #region Commands
        public ICommand TestPlaySoundCommand { get; private set; }
        public ICommand OpenSoundFileCommand { get; private set; }
        #endregion

        private void SetSoundFile(string eventLevel, string fileName)
        {
            var soundSetting = _settings.SoundSettings.FirstOrDefault(p => p.LevelName == eventLevel);
            if (soundSetting != null)
                soundSetting.FileName = fileName;
            RaisePropertiesChanged();
        }

        private string GetSoundFile(string eventLevel)
        {
            var soundSetting = _settings.SoundSettings.FirstOrDefault(p => p.LevelName == eventLevel);
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
                    if (_dialogService.OpenFileDialog() == true)
                    {
                        var intValue = Convert.ToInt32(level);
                        var fileName = _dialogService.FilePath;
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
