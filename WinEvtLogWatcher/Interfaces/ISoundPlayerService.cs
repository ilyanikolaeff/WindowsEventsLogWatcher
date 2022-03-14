using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    interface ISoundPlayerService
    {
        void AddEvent(EventEntry eventEntry);
        void AcknowledgeEvents();
        void ClearSoundQueue();
        void TryPlayLiveSignal();
    }
}
