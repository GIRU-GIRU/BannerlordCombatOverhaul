using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace GCO.Utility
{
    internal class HorseCrippleQueueItem
    {
        public HorseCrippleQueueItem(int agentIndex, MissionTime crippleTime)
        {
            AgentIndex = agentIndex;
            CrippleTime = crippleTime;
        }

        public int AgentIndex { get; set; }

        public MissionTime CrippleTime { get; set; }
    }
}
