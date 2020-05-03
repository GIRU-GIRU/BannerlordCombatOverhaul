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
        internal static partial class MeleeBalance
        {
            internal static bool GCOCheckHyperArmorConfiguration(Agent agent)
            {
                if (Config.ConfigSettings.HyperArmorEnabled && agent != null)
                {
                    {
                        return Config.ConfigSettings.HyperArmorEnabledForAllUnits || CheckIfAliveHero(agent);
                    }
                }

                return false;
            }

            internal static float GCOGetStaticFlinchPeriod(Agent attackerAgent, float defenderStunPeriod)
            {
                if (attackerAgent == Mission.Current.MainAgent)
                {
                    return 0.6f;
                };

                return defenderStunPeriod;
            }

            private static ConcurrentDictionary<Agent, MissionTime> hyperArmorCollection = new ConcurrentDictionary<Agent, MissionTime>();
            internal static void CreateHyperArmorBuff(Agent defenderAgent)
            {
                if (defenderAgent != null && defenderAgent.IsActive())
                {
                    hyperArmorCollection[defenderAgent] = MissionTime.MillisecondsFromNow((float)Config.ConfigSettings.HyperArmorDuration * 1000);
                }
            }

            internal static void CheckToAddHyperarmor(Agent agent, ref Blow b, ref Blow blow)
            {
                if (IsHyperArmorActive(agent))
                {
                    b.BlowFlag |= BlowFlags.ShrugOff;
                    blow.BlowFlag |= BlowFlags.ShrugOff;
                    if (agent == Mission.Current.MainAgent)
                    {
                        InformationManager.DisplayMessage(
                        new InformationMessage("Player hyperarmor prevented flinch!", Colors.White));
                    }
                }
            }

            private static bool IsHyperArmorActive(Agent agent)
            {
                if (hyperArmorCollection.ContainsKey(agent))
                {
                    if (hyperArmorCollection.TryGetValue(agent, out MissionTime hyperArmorDuration))
                    {
                        return !hyperArmorDuration.IsPast;
                    }
                }

                return false;
            }
        }
    }
}

