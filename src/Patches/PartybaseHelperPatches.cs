using Helpers;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace GCO.Features.ModdedWorldMapLogic
{
    //what the fuck is DoesSurrenderIsLogicalForParty mean taleworlds
    internal class PartyBaseHelperPatches
    {
        internal static void DoesSurrenderIsLogicalForPartyPostfix(ref bool __result, MobileParty ourParty, MobileParty enemyParty, float acceptablePowerRatio = 0.1f)
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
}
