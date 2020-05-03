using GCO.GCOMissionLogic;
using GCO.ModOptions;
using System;
using System.Collections.Concurrent;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace GCO.GCOToolbox
{
    internal static partial class GCOToolbox
    {
        private static bool CheckIfAliveHero(Agent agent)
        {
            bool isPlayer = false;

            if (agent != null && agent.IsActive())
            {
                if (agent.IsHuman && agent.IsHero)
                {
                    isPlayer = true;
                }
            }

            return isPlayer;
        }       
            
    }
}