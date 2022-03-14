using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    class SoundPlayerService : ISoundPlayerService
    {
        private readonly ConcurrentQueue<EventSound> _soundsQueue = new ConcurrentQueue<EventSound>();
        private readonly Settings _settingsProvider;
        private readonly EventLevelComparer _eventLevelComparer = new EventLevelComparer();
        private CancellationTokenSource _cancellationTokenSource;
        private bool _inProcessingQueue = false;
        public SoundPlayerService(Settings settingsProvider)
        {
            _settingsProvider = settingsProvider ?? throw new ArgumentNullException("settingsProvider");
        }

        public async Task ProcessEvent(EventEntry eventEntry)
        {
            var soundsSettings = _settingsProvider.SoundSettings;
            var eventLevel = (int)eventEntry.Level;
            var eventLevelName = _eventLevelComparer.GetLevelName(eventLevel);
            var soundSetting = soundsSettings.FirstOrDefault(p => p.LevelName == eventLevelName);
            var eventSound = new EventSound(soundSetting.FileName, soundSetting.IsCycle, eventLevel);

            _soundsQueue.Enqueue(eventSound);
            if (!_inProcessingQueue)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                await ProcessQueue(_cancellationTokenSource.Token);
            }
        }

        // логика обработки очереди звуковых сообщений
        private Task ProcessQueue(CancellationToken cancellationToken)
        {
            _inProcessingQueue = true;
            while (_soundsQueue.Count() > 0 || cancellationToken.IsCancellationRequested)
            {
                var result = _soundsQueue.TryDequeue(out EventSound eventSound);
                if (result)
                {
                    var soundPlayer = new SoundPlayer(eventSound.FileName);
                    Task.Run(() => PlaySound(soundPlayer, eventSound.IsCycle, cancellationToken), cancellationToken);
                }
            }
            _inProcessingQueue = false;
            _cancellationTokenSource = null;
            return Task.CompletedTask;
        }

        public void AcknowledgeEvents()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        private Task PlaySound(SoundPlayer soundPlayer, bool isCycle, CancellationToken cancellationToken)
        {
            if (!isCycle)
                return Task.Run(soundPlayer.Play, cancellationToken);
            else
                return Task.Run(soundPlayer.PlayLooping, cancellationToken);
        }
    }
}
