using Newtonsoft.Json;

namespace GCO.ModOptions
{
    public class ConfigSettings
    {
        [JsonProperty("HPOnKillEnabled")]
        public bool HPOnKillEnabled { get; set; }

        [JsonProperty("HPOnKillAmount")]
        public float HPOnKillAmount { get; set; }

        [JsonProperty("HPOnKillMedicineLevelScalePersentage")]
        public float HPOnKillMedicineLevelScalePersentage { get; set; }

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
    }
}
