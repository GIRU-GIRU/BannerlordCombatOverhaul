using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace GCO.Patches
{
    //pogchamp TGlees45 for 15 sub gift
    internal class BanditsCampaignBehaviorPatches
    {
        internal static void conversation_bandits_will_join_player_on_conditionPostfix(ref bool __result)
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
