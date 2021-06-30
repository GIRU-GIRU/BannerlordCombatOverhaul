using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace GCO.GCOMissionLogic
{
    public class HorseCrippleLogic : MissionLogic
    {

        private static ConcurrentDictionary<Agent, MissionTime> crippledHorseCollection = new ConcurrentDictionary<Agent, MissionTime>();

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
                        if (queueItem.Item1.IsActive())
                        {
                            var test = queueItem.Item1.AgentDrivenProperties.MountSpeed;
                            queueItem.Item1.UpdateAgentStats();

                        }
                    }
                }
            }
        }
        internal static bool CheckHorseCrippled(Agent agent)
        {
            if (crippledHorseCollection.ContainsKey(agent))
            {
                if (crippledHorseCollection.TryGetValue(agent, out MissionTime horseCrippleDuration))
                {
                    return !horseCrippleDuration.IsPast;
                }
            }

            return false;
        }
        internal static void CrippleHorseNew(Agent victim, MissionTime missionTime)
        {
            if (victim != null && victim.IsActive())
            {
                horseCrippleQueue.Enqueue(Tuple.Create(victim, missionTime));
                crippledHorseCollection[victim] = missionTime;
            }
        }
    }
}


