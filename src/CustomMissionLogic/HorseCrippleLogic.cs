using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace GCO.CustomMissionLogic
{
    public class HorseCrippleLogic : MissionLogic
    {
        public HorseCrippleLogic() : base()
        {

        }

        public static Queue<Tuple<Agent, MissionTime>> horseCrippleQueue = new Queue<Tuple<Agent, MissionTime>>();

        public override void OnMissionTick(float dt)
        {

            iterateQueue();

            base.OnMissionTick(dt);
        }

        private void iterateQueue()
        {
            if (horseCrippleQueue.Count > 0)
            {
                if (horseCrippleQueue.Peek().Item2.IsPast)
                {
                    var queueItem = horseCrippleQueue.Dequeue();

                    if (queueItem.Item1 != null)
                    {
                        try
                        {
                            if (queueItem.Item1.IsActive())
                            {
                                queueItem.Item1.AgentDrivenProperties.MountSpeed *= 8;
                                queueItem.Item1.UpdateAgentStats();
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
        }

        internal static void CrippleHorseNew(Agent victim, MissionTime missionTime)
        {
            horseCrippleQueue.Enqueue(Tuple.Create(victim, missionTime));
        }
    }
}


