using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TNV.LogWatcher.DataTransfer;

namespace TNV.LogWatcher.Manager
{
    class SoundPlayerService : ISoundPlayerService
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly ConcurrentQueue<EventSound> _soundsQueue = new ConcurrentQueue<EventSound>();
        private readonly Settings _settings;
        private readonly EventLevelComparer _eventLevelComparer = new EventLevelComparer();
        private SoundPlayer _currentSoundPlayer;
        private bool _currentlyPlayingSound = false;
        private bool _currentlyPlayingLoopSound = false;

        public SoundPlayerService(Settings settings)
        {
            _settings = settings;
        }

        public async Task Start()
        {
            await ProcessQueue();
        }
        private async Task ProcessQueue()
        {
            while (true)
            {
                await Task.Delay(1);

                if (_currentlyPlayingSound)
                    continue;

                var result = _soundsQueue.TryDequeue(out EventSound eventSound);
                if (result)
                {
                    if (!File.Exists(eventSound.FileName))
                        continue;
                    _currentSoundPlayer = new SoundPlayer(eventSound.FileName);
                    PlaySound(eventSound.IsCycle);
                }
            }
        }
        public void AddEvent(EventEntry eventEntry)
        {
            if (_soundsQueue.Count() > 10)
                return;

            var soundsSettings = _settings.SoundSettings;
            var eventLevel = (int)eventEntry.Level;
            var eventLevelName = _eventLevelComparer.GetLevelName(eventLevel);
            var soundSetting = soundsSettings.FirstOrDefault(p => p.LevelName == eventLevelName);

            // Если такой звук существует, то добавляем его в очередь проигрывания
            if (File.Exists(soundSetting.FileName))
            {
                var eventSound = new EventSound(soundSetting.FileName, soundSetting.IsCycle, eventLevel);
                _soundsQueue.Enqueue(eventSound);
                _logger.Info($"Added element to sounds queue (count = {_soundsQueue.Count()})");
            }
        }

        /// <summary>
        /// Квитировать все события (отмена текущего проигрывания)
        /// </summary>
        public void AcknowledgeEvents()
        {
            _logger.Info($"Called acknowledge events");
            if (_currentSoundPlayer != null)
            {
                _logger.Info($"Stop()");
                _currentSoundPlayer.Stop();
                _currentlyPlayingLoopSound = false;
            }
        }

        private void PlaySound(bool isCycle)
        {
            _currentlyPlayingSound = true;
            if (!isCycle)
            {
                _logger.Info($"Playing {_currentSoundPlayer.SoundLocation} file one time");
                _currentSoundPlayer.Play();
            }
            else
            {
                if (_currentlyPlayingLoopSound)
                    return;
                _currentlyPlayingLoopSound = true;
                _logger.Info($"Playing {_currentSoundPlayer.SoundLocation} in cycle");
                _currentSoundPlayer.PlayLooping();
            }
            _currentlyPlayingSound = false;
        }


        /// <summary>
        /// Очистка очереди звуковых сообщений (если в данный момент что-то проигрывается в цикле, то оно так и продолжит проигрываться в цикле до нажатия кнопки квитирования)
        /// Данная логика была реализована по требования Комарова А.А.
        /// </summary>
        public void ClearSoundQueue()
        {
            _logger.Info($"Called clear sound queue");
            _soundsQueue.ClearQueue();
        }

        public void TryPlayLiveSignal()
        {
            throw new NotImplementedException();
        }
    }
}
