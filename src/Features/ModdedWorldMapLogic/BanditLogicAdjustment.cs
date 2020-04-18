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
    [HarmonyPatch(typeof(PartyBaseHelper), "DoesSurrenderIsLogicalForParty")]
    public class BanditLogicAdjustment
    {
        private static void Postfix(ref bool __result, MobileParty ourParty, MobileParty enemyParty, float acceptablePowerRatio = 0.1f)
        {
            //enemy party strength + your roguery stat
            var enemyPartyStr = enemyParty.Party.CalculateStrength() + (int)Math.Floor((double)Hero.MainHero.GetSkillValue(DefaultSkills.Roguery));

            var banditPartyStr = ourParty.Party.CalculateStrength() * 20;

            __result = banditPartyStr < enemyPartyStr;
        }
    }

    //pogchamp TGlees45 for 15 sub gift
    [HarmonyPatch(typeof(BanditsCampaignBehavior), "conversation_bandits_will_join_player_on_condition")]
    public class IsSurrenderLogical
    {
        private static void Postfix(ref bool __result)
        {
            if (PartyBaseHelper.DoesSurrenderIsLogicalForParty(MobileParty.ConversationParty, MobileParty.MainParty))
            {
                __result = MobileParty.ConversationParty.Party.Random.GetValue(0) > 50;
            }
        }
    }

}
