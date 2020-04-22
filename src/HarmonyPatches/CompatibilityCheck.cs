using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace GCO.HarmonyPatches
{
    class CompatibilityCheck
    {
        public void CheckAndApply()
        {
            var methodBases = Harmony.GetAllPatchedMethods().ToList<MethodBase>();

            foreach (var method in methodBases)
            {
                if (Harmony.GetPatchInfo(method).Owners.Contains("xorberax.cutthrougheveryone"))
                {
                    ApplyCompatibility();
                }
            }
        }

        public void ApplyCompatibility()
        {
            Config.ConfigSettings.CleaveEnabled = false;
            Config.compatibilitySettings.xorbarexCleaveExists = true;
            InformationManager.DisplayMessage(new InformationMessage("Xorbarex Cut Through Everyone installation detected", Color.White));
        }
    }
}
