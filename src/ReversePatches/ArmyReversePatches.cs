using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace GCO.ReversePatches
{
    [HarmonyPatch]
    public static class ArmyReversePatches
    {

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(CampaignEventDispatcher), "OnArmyDispersed")]
        internal static void OnArmyDispersed(this CampaignEventDispatcher __instance, Army army, Army.ArmyDispersionReason reason, bool isPlayersArmy)
        {
            throw new NotImplementedException("Need to patch first");
        }

    }
}
