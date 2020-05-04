using System.IO;
using System.Reflection;
using GCO.GCOToolbox;
using HarmonyLib;
using Newtonsoft.Json;
using static GCO.ModOptions.SubModuleInfo;

namespace GCO.ModOptions
{
    public static class Config
    {
        private static readonly string ConfigFilePath =
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ModOptions/GCOconfig.json");

        private static readonly bool configExists = File.Exists(ConfigFilePath);
        public static bool ConfigLoadedSuccessfully { get; private set; }

        public static ConfigSettings ConfigSettings { get; private set; }

        public static CompatibilitySettings CompatibilitySettings { get; private set; }

        public static SubModuleContents SubModuleInfoContents { get; private set; }

    public static void InitConfig()
        {
            CompatibilitySettings = new CompatibilitySettings()
            {
                XorbarexCleaveExists = false,
            };

            SubModuleInfo info = new SubModuleInfo();
            SubModuleInfoContents = info.DeserializeSubModule();

            if (configExists)
            {
                try
                {
                    ConfigSettings = JsonConvert.DeserializeObject<ConfigSettings>(File.ReadAllText(ConfigFilePath));
                    ConfigLoadedSuccessfully = true;
                    return;
                }
                catch { }
            }

            ConfigLoadedSuccessfully = false;

            ConfigSettings = new ConfigSettings()
            {
                CleaveEnabled = true,
                AdditionalCleaveForTroopsInShieldWall = true,
                AdditionalCleaveForTroopsInShieldWallAngleRestriction = 60,
                CleaveEnabledForAllUnits = true,

                StandardizedFlinchOnEnemiesEnabled = true,

                HyperArmorEnabled = true,
                HyperArmorEnabledForAllUnits = true,
                HyperArmorDuration = 1,

                HPOnKillEnabled = true,
                HPOnKillForAI = true,
                HPOnKillMedicineLevelScalePercentage = 0.1f,
                HPOnKillAmount = 20f,

                ProjectileBalancingEnabled = true,
                ProjectileStunPercentageThreshold = 40f,
                HorseProjectileCrippleEnabled = true,
                HorseProjectileCrippleDuration = 2,

                OrderVoiceCommandQueuing = true,
                OrderControllerCameraImprovementsEnable = true,
                MurderEnabled = false,
                SimplifiedSurrenderLogic = true,
                TrueFriendlyFireEnabled = false,
            };

            
        }

        public static void ConfigureHarmonyPatches(Harmony harmony)
        {
            if (ConfigSettings.HyperArmorEnabled || ConfigSettings.ProjectileBalancingEnabled)
            {
                HarmonyPatchesConfiguration.HyperArmorAndProjectileBalancing(harmony);
            }

            //ConfigureQueuedVoiceLogic in SubModuleMain due to static class initializing incorrectly

            if (ConfigSettings.StandardizedFlinchOnEnemiesEnabled)
            {
                HarmonyPatchesConfiguration.StandardizedFlinchOnEnemiesEnablePatch(harmony);
            }

            if (ConfigSettings.TrueFriendlyFireEnabled || ConfigSettings.MurderEnabled)
            {
                HarmonyPatchesConfiguration.KillFriendliesPatch(harmony);
            }

            if (ConfigSettings.CleaveEnabled)
            {
                HarmonyPatchesConfiguration.CleaveEnabledPatch(harmony);
            }

            if (ConfigSettings.SimplifiedSurrenderLogic)
            {
                HarmonyPatchesConfiguration.SimplifiedSurrenderLogicEnabledPatch(harmony);
            }

            if (ConfigSettings.ProjectileBalancingEnabled)
            {
                HarmonyPatchesConfiguration.ProjectileBalancingEnabledPatch(harmony);
            }

            if (ConfigSettings.OrderControllerCameraImprovementsEnable)
            {
                HarmonyPatchesConfiguration.OrderControllerCameraImprovementsPatch(harmony);
            }       
        }
    }
}
