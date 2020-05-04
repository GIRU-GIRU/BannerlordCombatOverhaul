using Newtonsoft.Json;

namespace GCO.ModOptions
{
    public class ConfigSettings
    {
        [JsonProperty("HPOnKillEnabledForHeros")]
        public bool HPOnKillEnabledForHeros { get; set; }

        [JsonProperty("HPOnKillAmount")]
        public float HPOnKillAmount { get; set; }

        [JsonProperty("HPOnKillForAI")]
        public bool HPOnKillForAI { get; set; }

        [JsonProperty("HPOnKillMedicineLevelScalePercentage")]
        public float HPOnKillMedicineLevelScalePercentage { get; set; }

        [JsonProperty("CleaveEnabledForHeros")]
        public bool CleaveEnabled { get; set; }

        [JsonProperty("CleaveEnabledForAllUnits")]
        public bool CleaveEnabledForAllUnits { get; set; }

        [JsonProperty("SimplifiedSurrenderLogic")]
        public bool SimplifiedSurrenderLogic { get; set; }

        [JsonProperty("HyperArmorEnabledForHeros")]
        public bool HyperArmorEnabledForHeros { get; set; }

        [JsonProperty("HyperArmorEnabledForAllUnits")]
        public bool HyperArmorEnabledForAllUnits { get; set; }

        [JsonProperty("HyperArmorDuration")]
        public double HyperArmorDuration { get; set; }

        [JsonProperty("ProjectileBalancingEnabled")]
        public bool ProjectileBalancingEnabled { get; set; }

        [JsonProperty("ProjectileStunPercentageThreshold")]
        public float ProjectileStunPercentageThreshold { get; set; }

        [JsonProperty("StandardizedFlinchOnEnemiesEnabled")]
        public bool StandardizedFlinchOnEnemiesEnabled { get; set; }

        [JsonProperty("AdditionalCleaveForTroopsInShieldWall")]
        public bool AdditionalCleaveForTroopsInShieldWall { get; set; }

        [JsonProperty("AdditionalCleaveForTroopsInShieldWallMomentumLoss")]
        public int AdditionalCleaveForTroopsInShieldWallMomentumLoss { get; set; }

        [JsonProperty("AdditionalCleaveForTroopsInShieldWallAngleRestriction")]
        public int AdditionalCleaveForTroopsInShieldWallAngleRestriction { get; set; }

        [JsonProperty("OrderVoiceCommandQueuing")]
        public bool OrderVoiceCommandQueuing { get; set; }

        [JsonProperty("TrueFriendlyFireEnabled")]
        public bool TrueFriendlyFireEnabled { get; set; }

        [JsonProperty("MurderEnabled")]
        public bool MurderEnabled { get; set; }

        [JsonProperty("HorseProjectileCrippleEnabled")]
        public bool HorseProjectileCrippleEnabled { get; set; }

        [JsonProperty("HorseProjectileCrippleDuration")]
        public float HorseProjectileCrippleDuration { get; internal set; }

        [JsonProperty("OrderControllerCameraImprovementsEnable")]
        public bool OrderControllerCameraImprovementsEnable { get; set; }
    }
}
