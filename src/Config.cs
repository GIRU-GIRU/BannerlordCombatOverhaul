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

        [JsonProperty("ProjectileStunPercentageThreshold")]
        public float ProjectileStunPercentageThreshold { get; set; }

        [JsonProperty("EnableStandardizedFlinch")]
        public bool StandardizedFlinchOnEnemiesEnabled { get; set; }

        [JsonProperty("AdditionalCleaveForTroopsInShieldWall")]
        public bool AdditionalCleaveForTroopsInShieldWall { get; set; }

        [JsonProperty("OrderVoiceCommandQueuing")]
        public bool OrderVoiceCommandQueuing { get; set; }

        [JsonProperty("SwingThroughTeammatesEnabled")]
        public bool SwingThroughTeammatesEnabled { get; set; }
    }

    public static class Config
    {
        public static ConfigSettings ConfigSettings { get; set; }

        private static readonly string ConfigFilePath =
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "GCOconfig.json");

        private static readonly bool configExists = File.Exists(ConfigFilePath);

        public static bool ConfigLoadedSuccessfully { get; set; }

        public static bool xorbarexCleaveExists { get; set; }

        public static void initConfig()
        {
            if (configExists)
            {
                try
                {
                    ConfigSettings = JsonConvert.DeserializeObject<ConfigSettings>(File.ReadAllText(ConfigFilePath));
                    ConfigLoadedSuccessfully = true;
                    return;
                }
                catch
                {

                }
            }

            ConfigLoadedSuccessfully = false;
            ConfigSettings = new ConfigSettings();

            ConfigSettings.CleaveEnabled = true;
            ConfigSettings.HPOnKillEnabled = true;
            ConfigSettings.HyperArmorEnabled = true;
            ConfigSettings.SimplifiedSurrenderLogic = true;
            ConfigSettings.HPOnKillAmount = 20f;
            ConfigSettings.ProjectileStunPercentageThreshold = 40f;
            ConfigSettings.HyperArmorDuration = 1f;
            ConfigSettings.StandardizedFlinchOnEnemiesEnabled = true;
            ConfigSettings.AdditionalCleaveForTroopsInShieldWall = true;
            ConfigSettings.OrderVoiceCommandQueuing = true;
            ConfigSettings.SwingThroughTeammatesEnabled = false;
        }

        public static void ConfigureHarmonyPatches(ref Harmony harmony)
        {
            var harmonyPatchConfig = new HarmonyPatchingConfiguration();

            //Hyperarmor & Projectile flinch, two features so not manually patched
            //var getDefendCollisionResultsAux = typeof(Mission).GetMethod("GetDefendCollisionResultsAux");
            //var registerBlow = typeof(Mission).GetMethod("RegisterBlow");


            if (ConfigSettings.OrderVoiceCommandQueuing)
            {
                harmonyPatchConfig.OrderVoiceCommandQueuingPatch(ref harmony);  
            }

     
            if (ConfigSettings.StandardizedFlinchOnEnemiesEnabled)
            {
                harmonyPatchConfig.StandardizedFlinchOnEnemiesEnablePatch(ref harmony);
            }

            if (ConfigSettings.SwingThroughTeammatesEnabled)
            {
                harmonyPatchConfig.SwingThroughTeammatesEnabledPatch(ref harmony);
            }

        
            if (ConfigSettings.CleaveEnabled)
            {
                harmonyPatchConfig.CleaveEnabledPatch(ref harmony);
            }

    
            if (ConfigSettings.SimplifiedSurrenderLogic)
            {
                harmonyPatchConfig.SimplifiedSurrenderLogicEnabledPatch(ref harmony);
            }
        }
    }
}
