using GCO.Utility;
using System;
using TaleWorlds.MountAndBlade;
using static TaleWorlds.MountAndBlade.SkinVoiceManager;

namespace GCO.Features.CustomMissionLogic
{
    internal class QueuedVoiceLogic : MissionLogic
    {
        public QueuedVoiceLogic() : base()
        {
            VoiceCommandQueue.ResetVoiceCommandTimer(10f);
        }

        public override void OnMissionTick(float dt)
        {
            var queue = VoiceCommandQueue.GetVoiceCommandQueue();
            if (queue.Count > 0)
            {
                var timer = VoiceCommandQueue.GetVoiceCommandTimer();

                if (timer.IsPast)
                {
                    var queueItem = VoiceCommandQueue.GetNextQueueItem();
                    var voiceType = new SkinVoiceType(queueItem.VoiceTypeString);
             
                    Agent.Main.MakeVoice(voiceType, CombatVoiceNetworkPredictionType.NoPrediction);
                    VoiceCommandQueue.ResetVoiceCommandTimer(queueItem.DelayAfter);

                }
            }
            base.OnMissionTick(dt);
        }
    }
}
