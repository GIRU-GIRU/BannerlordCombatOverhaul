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

        public static Queue<Tuple<int, MissionTime>> horseCrippleQueue = new Queue<Tuple<int, MissionTime>>();

        //public static List<Queue<Tuple<int, MissionTime>>> list = new List<Queue<Tuple<int, MissionTime>>>();

        // public static ConcurrentBag<Queue<Tuple<int, MissionTime>>> bag = new ConcurrentBag<Queue<Tuple<int, MissionTime>>>();

        public static ConcurrentDictionary<Guid, Queue<Tuple<int, MissionTime>>> dictionary = new ConcurrentDictionary<Guid, Queue<Tuple<int, MissionTime>>>();

        public override void OnMissionTick(float dt)
        {

            if (dictionary.Count > 0)
            {
                iterateDictionary();
            }
            else
            {
                iterateQueue();
            }


            base.OnMissionTick(dt);
        }

        private void iterateQueue()
        {

            if (horseCrippleQueue.Count > 0)
            {
                if (horseCrippleQueue.Peek().Item2.IsPast)
                {
                    var queueItem = horseCrippleQueue.Dequeue();

                    var agent = Mission.AllAgents.FirstOrDefault(x => x.Index == queueItem.Item1);
                    if (agent != null)
                    {
                        agent.AgentDrivenProperties.MountSpeed *= 8;
                        agent.UpdateAgentStats();
                    }
                }
            }
        }

        private void iterateDictionary()
        {
            if (!dictionary.IsEmpty)
            {
                foreach (var item in dictionary)
                {
                    Queue<Tuple<int, MissionTime>> q;
                    var success = dictionary.TryGetValue(item.Key, out q);

                    if (success)
                    {
                        if (q.Count > 0)
                        {
                            var agentinfo = q.Dequeue();

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
            horseCrippleQueue.Enqueue(Tuple.Create(index, missionTime));

            if (horseCrippleQueue.Count > 10)
            {

                var notAdded = true;

                while (notAdded)
                {
                    notAdded = !dictionary.TryAdd(Guid.NewGuid(), horseCrippleQueue);              
                }
                horseCrippleQueue.Clear();


            }
        }
    }
}


