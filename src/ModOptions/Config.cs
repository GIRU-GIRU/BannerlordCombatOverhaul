using System.IO;
using System.Reflection;
using HarmonyLib;
using Newtonsoft.Json;

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

        public static void InitConfig()
        {
            CompatibilitySettings = new CompatibilitySettings()
            {
                XorbarexCleaveExists = false,
            };

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
                ProjectileBalancingEnabled = true,
                HPOnKillEnabled = true,
                HPOnKillMedicineLevelScalePercentage = 0.1f,
                HyperArmorEnabled = true,
                SimplifiedSurrenderLogic = true,
                HPOnKillAmount = 20f,
                ProjectileStunPercentageThreshold = 40f,
                HyperArmorDuration = 1f,
                StandardizedFlinchOnEnemiesEnabled = true,
                AdditionalCleaveForTroopsInShieldWall = true,
                AdditionalCleaveForTroopsInShieldWallAngleRestriction = 60,
                OrderVoiceCommandQueuing = true,
                HorseProjectileCrippleEnabled = true,
                HorseProjectileCrippleDuration = 2,

                MurderEnabled = false,
                TrueFriendlyFireEnabled = false,
            };
        }

        public static void ConfigureHarmonyPatches(ref Harmony harmony)
        {
            if (ConfigSettings.HyperArmorEnabled || ConfigSettings.ProjectileBalancingEnabled)
            {
                HarmonyPatchesConfiguration.HyperArmorAndProjectileBalancing(ref harmony);
            }

            if (ConfigSettings.OrderVoiceCommandQueuing)
            {
                HarmonyPatchesConfiguration.OrderVoiceCommandQueuingPatch(ref harmony);
            }

            if (ConfigSettings.StandardizedFlinchOnEnemiesEnabled)
            {
                HarmonyPatchesConfiguration.StandardizedFlinchOnEnemiesEnablePatch(ref harmony);
            }

            if (ConfigSettings.TrueFriendlyFireEnabled || ConfigSettings.MurderEnabled)
            {
                HarmonyPatchesConfiguration.KillFriendliesPatch(ref harmony);
            }

            if (ConfigSettings.CleaveEnabled)
            {
                HarmonyPatchesConfiguration.CleaveEnabledPatch(ref harmony);
            }

            if (ConfigSettings.SimplifiedSurrenderLogic)
            {
                HarmonyPatchesConfiguration.SimplifiedSurrenderLogicEnabledPatch(ref harmony);
            }

            if (ConfigSettings.ProjectileBalancingEnabled)
            {
                HarmonyPatchesConfiguration.ProjectileBalancingEnabledPatch(ref harmony);
            }
        }
    }
}
