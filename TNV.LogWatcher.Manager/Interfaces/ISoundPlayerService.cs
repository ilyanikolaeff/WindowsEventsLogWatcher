using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNV.LogWatcher.DataTransfer;

namespace TNV.LogWatcher.Manager
{
    interface ISoundPlayerService
    {
        void AddEvent(EventEntry eventEntry);
        void AcknowledgeEvents();
        void ClearSoundQueue();
        void TryPlayLiveSignal();
    }
}
