using HarmonyLib;
using System.Collections.ObjectModel;
using TaleWorlds.MountAndBlade;

namespace GCO.ReversePatches
{
    // TODO: Rework with Harmony Reverse Patches
    internal static class OrderControllerReversePatches
    {
        internal static ObservableCollection<Formation> GetSelectedFormations(this OrderController __instance)
        {
            return Traverse.Create(__instance).Field<ObservableCollection<Formation>>("selectedFormations").Value;
        }

        internal static Team GetTeam(this OrderController __instance)
        {
            return Traverse.Create(__instance).Field<Team>("team").Value;
        }
    }
}
