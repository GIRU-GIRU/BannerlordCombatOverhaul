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

        }

        public override void OnMissionTick(float dt)
        {
            // this isnt needed, right?
            //bool initialized = false;
            //MissionTime missionTime;

            //if (MissionTime.Now != null)
            //{
            //    initialized = true;
            //}

            //if (initialized)
            //{
            //    missionTime = MissionTime.SecondsFromNow(5);
            //    if (missionTime.IsPast)
            //    {
            //        Agent.Main.MakeVoice(VoiceType.Move, CombatVoiceNetworkPredictionType.NoPrediction);

            //    }
            //}

            var queue = QueueClass.GetVoiceCommandQueue();
            if (queue.Count > 0)
            {
                var timer = QueueClass.GetVoiceCommandTimer();

                if (timer.IsPast)
                {
                    var voiceTypeString = QueueClass.GetNextQueueItem();
                    var voiceType = new SkinVoiceType(voiceTypeString);
                    Agent.Main.MakeVoice(voiceType, CombatVoiceNetworkPredictionType.NoPrediction);

                }
            }
            base.OnMissionTick(dt);
        }
    }
}
