using GCO.ModOptions;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace GCO.Utility
{
    internal static class CompatibilityCheck
    {
        public static void CheckAndApply()
        {
            var methodBases = Harmony.GetAllPatchedMethods();

            foreach (var method in methodBases)
            {
                if (Harmony.GetPatchInfo(method).Owners.Contains("xorberax.cutthrougheveryone"))
                {
                    ApplyCompatibility();
                }
            }
        }

        private static void ApplyCompatibility()
        {
            Config.ConfigSettings.CleaveEnabled = false;
            Config.CompatibilitySettings.XorbarexCleaveExists = true;
            InformationManager.DisplayMessage(new InformationMessage("Xorbarex Cut Through Everyone installation detected", Color.White));
        }
    }
}
