using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;
using static TaleWorlds.MountAndBlade.SkinVoiceManager;

namespace GCO.Features.ModdedMissionLogic
{
    internal class QueuedVoiceLogic : MissionLogic
    {
        public QueuedVoiceLogic() : base()
        {
            QueueClass.ResetVoiceCommandTimer(10f);
        }

        public override void OnMissionTick(float dt)
        {
            var queue = QueueClass.GetVoiceCommandQueue();
            if (queue.Count > 0)
            {
                var timer = QueueClass.GetVoiceCommandTimer();

                if (timer.IsPast)
                {
                    var queueItem = QueueClass.GetNextQueueItem();
                    var voiceType = new SkinVoiceType(queueItem.VoiceTypeString);
                    Agent.Main.MakeVoice(voiceType, CombatVoiceNetworkPredictionType.NoPrediction);
                    QueueClass.ResetVoiceCommandTimer(queueItem.DelayAfter);

                }
            }
            base.OnMissionTick(dt);
        }
    }
}
