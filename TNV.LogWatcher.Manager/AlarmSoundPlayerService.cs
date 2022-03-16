using NAudio.Wave;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TNV.LogWatcher.DataTransfer;

namespace TNV.LogWatcher.Manager
{
    class AlarmSoundPlayerService : ISoundPlayerService
    {
        private readonly ConcurrentQueue<EventSound> _soundsQueue = new ConcurrentQueue<EventSound>();
        private readonly Settings _settings;
        private readonly EventLevelComparer _eventLevelComparer = new EventLevelComparer();
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IWavePlayer _wavePlayer = new WaveOutEvent();
        private WaveLoopStream _waveLoopStream;

        public AlarmSoundPlayerService(Settings settings)
        {
            _settings = settings;
        }

        public void AcknowledgeEvents()
        {
            //if (_waveLoopStream != null)
            //    _waveLoopStream.EnableLooping = false;
            _wavePlayer.Stop();
            //if ((_wavePlayer as WaveOutEvent).)
        }

        public void AddEvent(EventEntry eventEntry)
        {
            if (_soundsQueue.Count() >= 10)
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

        public void ClearSoundQueue()
        {
            _soundsQueue.ClearQueue();
            _logger.Info($"SoundsQueue cleared! (Count = {_soundsQueue.Count()})");
        }

        private void ProcessSoundsQueue()
        {
            while (true)
            {
                if (!_soundsQueue.IsEmpty)
                {
                    var tryDequeue = _soundsQueue.TryDequeue(out EventSound result);
                    if (tryDequeue)
                    {
                        try
                        {
                            _logger.Info($"Trying to play sound: fileName = {result.FileName}, isCycle = {result.IsCycle}");
                            TryPlaySound(result.FileName, result.IsCycle);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"Playing sound error: {ex}");
                        }
                    }
                }
            }
        }

        private void TryPlaySound(string fileName, bool isCycle)
        {
            if (_wavePlayer.PlaybackState != PlaybackState.Playing)
                ReadAndPlay(fileName, isCycle);
        }

        private void ReadAndPlay(string fileName, bool isCycle)
        {
            if (!isCycle)
            {
                var audioFileReader = new AudioFileReader(fileName);
                _wavePlayer.Init(audioFileReader);
            }
            else
            {
                var waveFileReader = new WaveFileReader(fileName);
                _waveLoopStream = new WaveLoopStream(waveFileReader);
                _wavePlayer.Init(_waveLoopStream);
            }
            _wavePlayer.Play();
        }

        public void Start()
        {
            Task.Run(ProcessSoundsQueue);
        }

        public void TryPlayLiveSignal()
        {
            if (_settings.EnableMonitoringSound)
            {
                if (_wavePlayer.PlaybackState != PlaybackState.Playing)
                {
                    var waveFileReader = new WaveFileReader(Properties.Resources.Monitoring);
                    _waveLoopStream = new WaveLoopStream(waveFileReader);
                    _wavePlayer.Init(_waveLoopStream);
                    _wavePlayer.Play();
                }
            }
        }
    }
}
