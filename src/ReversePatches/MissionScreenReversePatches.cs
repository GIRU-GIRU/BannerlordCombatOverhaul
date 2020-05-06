using HarmonyLib;
using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Engine;
using static HarmonyLib.AccessTools;
using System.Reflection;
using TaleWorlds.MountAndBlade.View.Screen;

namespace GCO.ReversePatches
{

    [HarmonyPatch]
    public static class MissionScreenReversePatches
    {
      

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(MissionScreen), "UpdateCamera")]
        internal static void UpdateCamera(this MissionScreen Mission, float dt)
        {
            throw new NotImplementedException("Need to patch first");
        }

      
    }

}

