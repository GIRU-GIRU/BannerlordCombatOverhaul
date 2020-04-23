using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GCO.Features.ModdedMissionLogic;
using GCO.Features.ModdedWorldMapLogic;
using GCO.HarmonyPatches;
using HarmonyLib;
using Helpers;
using Newtonsoft.Json;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace GCO
{
    public class ConfigSettings
    {
        [JsonProperty("HPOnKillEnabled")]
        public bool HPOnKillEnabled { get; set; }

        [JsonProperty("HPOnKillAmount")]
        public float HPOnKillAmount { get; set; }

        [JsonProperty("CleaveEnabled")]
        public bool CleaveEnabled { get; set; }

        [JsonProperty("SimplifiedSurrenderLogic")]
        public bool SimplifiedSurrenderLogic { get; set; }

        [JsonProperty("HyperArmorEnabled")]
        public bool HyperArmorEnabled { get; set; }

        [JsonProperty("HyperArmorDuration")]
        public float HyperArmorDuration { get; set; }

        [JsonProperty("ProjectileBalancingEnabled")]
        public bool ProjectileBalancingEnabled { get; set; }

        [JsonProperty("ProjectileStunPercentageThreshold")]
        public float ProjectileStunPercentageThreshold { get; set; }

        [JsonProperty("StandardizedFlinchOnEnemiesEnabled")]
        public bool StandardizedFlinchOnEnemiesEnabled { get; set; }

        [JsonProperty("AdditionalCleaveForTroopsInShieldWall")]
        public bool AdditionalCleaveForTroopsInShieldWall { get; set; }

        [JsonProperty("OrderVoiceCommandQueuing")]
        public bool OrderVoiceCommandQueuing { get; set; }

        [JsonProperty("TrueFriendlyFireEnabled")]
        public bool TrueFriendlyFireEnabled { get; set; }

        [JsonProperty("MurderEnabled")]
        public bool MurderEnabled { get; set; }
    }

    public class CompatibilitySettings
    {
        public bool XorbarexCleaveExists { get; set; }
    }

    public static class Config
    {
        private static readonly string ConfigFilePath =
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "GCOconfig.json");

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
                HyperArmorEnabled = true,
                SimplifiedSurrenderLogic = true,
                HPOnKillAmount = 20f,
                ProjectileStunPercentageThreshold = 40f,
                HyperArmorDuration = 1f,
                StandardizedFlinchOnEnemiesEnabled = true,
                AdditionalCleaveForTroopsInShieldWall = true,
                OrderVoiceCommandQueuing = true,

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

            if(ConfigSettings.ProjectileBalancingEnabled)
            {
                HarmonyPatchesConfiguration.ProjectileBalancingEnabledPatch(ref harmony);
            }
        }
    }
}
