using HarmonyLib;
using Helpers;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace GCO.Features.ModdedWorldMapLogic
{

    //what the fuck is DoesSurrenderIsLogicalForParty mean taleworlds
    public class SurrenderLogicAdjustment
    {
        private static void DoesSurrenderIsLogicalForPartyPostfix(ref bool __result, MobileParty ourParty, MobileParty enemyParty, float acceptablePowerRatio = 0.1f)
        {
            int enemyHeroRoguery = 0;
            int ourHeroRoguery = 0;
         
            if (!enemyParty.IsLeaderless)
            {
                enemyHeroRoguery = (int)Math.Floor((double)enemyParty.LeaderHero.GetSkillValue(DefaultSkills.Roguery));
            }
            if (!ourParty.IsLeaderless)
            {
                ourHeroRoguery = (int)Math.Floor((double)enemyParty.LeaderHero.GetSkillValue(DefaultSkills.Roguery));
            }

            var enemyPartyRogueryAdvantage = enemyHeroRoguery - ourHeroRoguery;

            var enemyPartyStr = enemyParty.Party.CalculateStrength() + enemyPartyRogueryAdvantage;
            var banditPartyStr = ourParty.Party.CalculateStrength() * 20;

            __result = banditPartyStr < enemyPartyStr;
        }
    }

    //pogchamp TGlees45 for 15 sub gift
    public class IsSurrenderLogical
    {
        private static void conversation_bandits_will_join_player_on_conditionPostfix(ref bool __result)
        {
            if (Config.ConfigSettings.SimplifiedSurrenderLogic)
            {
                if (PartyBaseHelper.DoesSurrenderIsLogicalForParty(MobileParty.ConversationParty, MobileParty.MainParty))
                {
                    __result = MobileParty.ConversationParty.Party.Random.GetValue(0) > 50;
                }
            }
       
        }
    }
}
