using System;
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
        public override void OnMissionTick(float dt)
        {
            if (horseCrippleQueue.Count > 0)
            {
                if (horseCrippleQueue.Peek().Item2.IsPast)
                {
                    var queueItem = horseCrippleQueue.Dequeue();

                    var agent = Mission.AllAgents.Where(x => x.Index == queueItem.Item1).FirstOrDefault();
                    if (agent != null)
                    {
                        agent.AgentDrivenProperties.MountSpeed *= 8;
                        agent.UpdateAgentStats();
                    }
                }
            }


            base.OnMissionTick(dt);
        }
    }
}
