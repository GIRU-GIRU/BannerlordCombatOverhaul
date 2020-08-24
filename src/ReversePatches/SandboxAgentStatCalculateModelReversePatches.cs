using HarmonyLib;
using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace GCO.ReversePatches
{

    [HarmonyPatch]
    internal static class SandboxAgentStatCalculateModelReversePatches
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(SandboxAgentStatCalculateModel), "UpdateHumanStats")]
        internal static void UpdateHumanStats(this SandboxAgentStatCalculateModel model, Agent agent, AgentDrivenProperties agentDrivenProperties)
        {
            throw new NotImplementedException("Need to patch first");
        }
      
    }
}
