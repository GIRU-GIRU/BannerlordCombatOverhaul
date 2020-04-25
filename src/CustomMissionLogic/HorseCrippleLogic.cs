using GCO.Utility;
using System.Linq;
using TaleWorlds.MountAndBlade;

namespace GCO.CustomMissionLogic
{
    public class HorseCrippleLogic : MissionLogic
    {
        public HorseCrippleLogic() : base()
        {
            horseCrippleQueue = new ConcurrentDictionaryOfQueues<HorseCrippleQueueItem>(10, UncrippleHorse, ShouldUncrippleHorse);
        }

        private readonly ConcurrentDictionaryOfQueues<HorseCrippleQueueItem> horseCrippleQueue;

        public override void OnMissionTick(float dt)
        {
            horseCrippleQueue.ProcessNext();

            base.OnMissionTick(dt);
        }

        internal void CrippleHorse(int index, MissionTime missionTime)
        {
            horseCrippleQueue.AddToNextAvailableQueue(new HorseCrippleQueueItem(index, missionTime));
        }

        /// <summary>
        /// This method is passed as a delegate Action into a Queue to keep the logic separated (queue should only process itself, and not do any game logic)
        /// </summary>
        private void UncrippleHorse(HorseCrippleQueueItem horseCrippleQueueItem)
        {
            var agent = Mission.AllAgents.FirstOrDefault(x => x.Index == horseCrippleQueueItem.AgentIndex);
            if (agent != null)
            {
                agent.AgentDrivenProperties.MountSpeed *= 8;
                agent.UpdateAgentStats();
            }
        }

        private bool ShouldUncrippleHorse(HorseCrippleQueueItem horseCrippleQueueItem)
        {
            return horseCrippleQueueItem.CrippleTime.IsPast;
        }
    }
}


