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

        private static Queue<Tuple<int, MissionTime>> currentQueue = new Queue<Tuple<int, MissionTime>>();

        private static ConcurrentDictionary<Guid, Queue<Tuple<int, MissionTime>>> dictionary = new ConcurrentDictionary<Guid, Queue<Tuple<int, MissionTime>>>();

        public override void OnMissionTick(float dt)
        {

            if (dictionary.Count > 0)
            {
                IterateDictionary();
            }
            else
            {
                IterateQueue();
            }


            base.OnMissionTick(dt);
        }

        private void IterateQueue()
        {
            if (currentQueue.Count > 0)
            {
                if (currentQueue.Peek().Item2.IsPast)
                {
                    var queueItem = currentQueue.Dequeue();

                    var agent = Mission.AllAgents.FirstOrDefault(x => x.Index == queueItem.Item1);
                    if (agent != null)
                    {
                        agent.AgentDrivenProperties.MountSpeed *= 8;
                        agent.UpdateAgentStats();
                    }
                }
            }
        }

        private void IterateDictionary()
        {
            if (!dictionary.IsEmpty)
            {
                foreach (var item in dictionary)
                {
                    var success = dictionary.TryGetValue(item.Key, out Queue<Tuple<int, MissionTime>> queue);

                    if (success)
                    {
                        if (queue.Count > 0)
                        {
                            var agentinfo = queue.Dequeue();

                            var agent = Mission.AllAgents.FirstOrDefault(x => x.Index == agentinfo.Item1);

                            if (agent != null)
                            {
                                agent.AgentDrivenProperties.MountSpeed *= 8;
                                agent.UpdateAgentStats();
                            }
                        }
                        else
                        {
                            dictionary.TryRemove(item.Key, out _);
                        }

                    }
                }
            }

        }

        internal static void CrippleHorse(int index, MissionTime missionTime)
        {
            if (currentQueue.Count < 10)
            {
                currentQueue.Enqueue(Tuple.Create(index, missionTime));
            }
            else
            {
                var queue = new Queue<Tuple<int, MissionTime>>(currentQueue);
                var notAdded = true;

                while (notAdded)
                {
                    notAdded = !dictionary.TryAdd(Guid.NewGuid(), queue);
                }
                currentQueue = new Queue<Tuple<int, MissionTime>>();
            }
        }
    }
}


