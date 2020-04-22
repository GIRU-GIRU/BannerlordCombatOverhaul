using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCO.Features.ModdedMissionLogic;
using GCO.Features.ModdedWorldMapLogic;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.MountAndBlade;

namespace GCO.HarmonyPatches
{
    class HarmonyPatchingConfiguration
    {
        internal void CleaveEnabledPatch(ref Harmony harmony)
        {
            var decideWeaponCollisionReaction = typeof(Mission).GetMethod("DecideWeaponCollisionReaction");
            var DecideWeaponCollisionReactionPostfix = typeof(PlayerCleaveLogic).GetMethod("DecideWeaponCollisionReactionPostfix");

            var meleeHitCallback = typeof(Mission).GetMethod("MeleeHitCallback");
            var meleeHitCallbackPostfix = typeof(PlayerCleaveLogic).GetMethod("MeleeHitCallbackPostfix");

            harmony.Patch(decideWeaponCollisionReaction, null, new HarmonyMethod(DecideWeaponCollisionReactionPostfix), null);
            harmony.Patch(meleeHitCallback, null, new HarmonyMethod(meleeHitCallbackPostfix), null);
        }

        internal void SimplifiedSurrenderLogicEnabledPatch(ref Harmony harmony)
        {
            var doesSurrenderIsLogicalForParty = typeof(PartyBaseHelper).GetMethod("DoesSurrenderIsLogicalForParty");
            var doesSurrenderIsLogicalForPartyPostfix = typeof(SurrenderLogicAdjustment).GetMethod("doesSurrenderIsLogicalForPartyPostfix");


            var conversation_bandits_will_join_player_on_condition = typeof(BanditsCampaignBehavior).GetMethod("MeleeHitCallback");
            var conversation_bandits_will_join_player_on_conditionPostfix = typeof(IsSurrenderLogical)
                                                                                .GetMethod("conversation_bandits_will_join_player_on_conditionPostfix");

            harmony.Patch(doesSurrenderIsLogicalForParty, null, new HarmonyMethod(doesSurrenderIsLogicalForPartyPostfix), null);
            harmony.Patch(conversation_bandits_will_join_player_on_condition, null,
                new HarmonyMethod(conversation_bandits_will_join_player_on_conditionPostfix), null);
        }

        internal void StandardizedFlinchOnEnemiesEnablePatch(ref Harmony harmony)
        {
            var createBlow = typeof(Mission).GetMethod("CreateBlow");
            var createBlowPrefix = typeof(FlinchManagement).GetMethod("CreateBlowPrefix");
            harmony.Patch(createBlow, new HarmonyMethod(createBlowPrefix), null, null, null);
        }

        internal void SwingThroughTeammatesEnabledPatch(ref Harmony harmony)
        {
            var cancelsDamageAndBlocksAttackBecauseOfNonEnemyCase = typeof(Mission).GetMethod("CancelsDamageAndBlocksAttackBecauseOfNonEnemyCase");

            var cancelsDamageAndBlocksAttackBecauseOfNonEnemyCasePrefix = typeof(PlayerCleaveLogic).GetMethod("CancelsDamageAndBlocksAttackBecauseOfNonEnemyCasePrefix");

            harmony.Patch(cancelsDamageAndBlocksAttackBecauseOfNonEnemyCase,
                new HarmonyMethod(cancelsDamageAndBlocksAttackBecauseOfNonEnemyCasePrefix), null, null, null);
        }

        internal void OrderVoiceCommandQueuingPatch(ref Harmony harmony)
        {
            var selectFormationMakeVoice = typeof(OrderController).GetMethod("SelectFormationMakeVoice");
            var SelectFormationMakeVoicePrefix = typeof(ModdedOrderVoiceCommands).GetMethod("SelectFormationMakeVoicePrefix");

            var afterSetOrderMakeVoice = typeof(OrderController).GetMethod("AfterSetOrderMakeVoice");
            var afterSetOrderMakeVoicePrefix = typeof(ModdedOrderVoiceCommands).GetMethod("AfterSetOrderMakeVoicePrefix");

            harmony.Patch(selectFormationMakeVoice, new HarmonyMethod(SelectFormationMakeVoicePrefix), null, null, null);
            harmony.Patch(afterSetOrderMakeVoice, new HarmonyMethod(afterSetOrderMakeVoicePrefix), null, null, null);
        }
    }
}
