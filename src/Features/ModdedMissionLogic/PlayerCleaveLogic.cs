using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace GCO.Features.ModdedMissionLogic
{
    internal static class PlayerCleaveLogic
    {
        private static void DecideWeaponCollisionReactionPostfix(Mission __instance, Blow registeredBlow, ref AttackCollisionData collisionData, Agent attacker, Agent defender, bool isFatalHit, bool isShruggedOff, ref MeleeCollisionReaction colReaction)
        {
            if (Config.ConfigSettings.CleaveEnabled)
            {
                if (PlayerCleaveLogicExtensionMethods.CheckApplyCleave(__instance, attacker, defender, registeredBlow, isShruggedOff))
                {
                    colReaction = MeleeCollisionReaction.SlicedThrough;
                }
            }
        }

        private static bool CancelsDamageAndBlocksAttackBecauseOfNonEnemyCasePrefix(ref bool __result, Agent attacker, Agent victim)
		{
            if (attacker != Mission.Current.MainAgent)
            {
                if (victim == null || attacker == null)
                {
                    return false;
                }
                bool flag = !GameNetwork.IsSessionActive || (MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) <= 0 && MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) <= 0) || Mission.Current.Mode == MissionMode.Duel || attacker.Controller == Agent.ControllerType.AI;
                bool flag2 = attacker.IsFriendOf(victim);
                __result = (flag && flag2) || (victim.IsHuman && !flag2 && !attacker.IsEnemyOf(victim));
            }

            __result = false;
            return false;		
		}

        private static void MeleeHitCallbackPostfix(Mission __instance, ref AttackCollisionData collisionData, Agent attacker, Agent victim, GameEntity realHitEntity, float momentumRemainingToComputeDamage, ref float inOutMomentumRemaining, ref MeleeCollisionReaction colReaction, CrushThroughState cts, Vec3 blowDir, Vec3 swingDir, bool crushedThroughWithoutAgentCollision)
        {
            if (Config.ConfigSettings.CleaveEnabled)
            {
                if (PlayerCleaveLogicExtensionMethods.CheckApplyCleave(__instance, attacker, victim, colReaction))
                {
                    if (attacker.HasMount)
                    {
                        inOutMomentumRemaining = momentumRemainingToComputeDamage * 0.25f;
                    }
                    else if(PlayerCleaveLogicExtensionMethods.IsDefenderAFriendlyInShieldFormation(attacker, victim))
                    {
                        inOutMomentumRemaining = momentumRemainingToComputeDamage;
                    }
                    else
                    {
                        inOutMomentumRemaining = momentumRemainingToComputeDamage * 0.5f;
                    }
                }
            }
        }
	}

    internal static class PlayerCleaveLogicExtensionMethods
    {
        internal static bool IsDefenderAFriendlyInShieldFormation(Agent attacker, Agent defender)
        {
            return Config.ConfigSettings.AdditionalCleaveForTroopsInShieldWall 
                && defender.Formation != null
                && defender.Formation.ArrangementOrder == ArrangementOrder.ArrangementOrderShieldWall
                && ((attacker.Formation != null && attacker.Formation.ArrangementOrder == ArrangementOrder.ArrangementOrderShieldWall) || attacker.IsPlayerControlled) // for some reason, player is always considered to be in a Line formation
                && attacker.Team == defender.Team
                && IsAttackerFacingSimilarDirection(attacker, defender);
        }

        /// <summary>
        /// Current check returns true if attacker is looking roughly over the shoulder of the defender
        /// </summary>
        private static bool IsAttackerFacingSimilarDirection(Agent attacker, Agent defender)
        {
            float maxAngle = 1f; // this math is done in radians (1 radian = ~57°)
            // the smaller angleBetween is, the more Look Direction of the agents is simillar
            var angleBetween = attacker.LookDirection.AsVec2.AngleBetween(defender.LookDirection.AsVec2);
            return angleBetween <= maxAngle;
        }

        internal static bool CheckApplyCleave(Mission __instance, Agent attacker, Agent defender, Blow registeredBlow, bool isShruggedOff)
        {
            bool shouldCleave = false;

            if (attacker != null && defender != null)
            {
                if (attacker.IsHero && !attacker.HasMount)
                {
                    if (!CancelsDamageAndBlocksAttackBecauseOfNonEnemyCase(__instance, attacker, defender))
                    {
                        if (registeredBlow.DamageType != DamageTypes.Blunt && !isShruggedOff)
                        {
                            shouldCleave = true;
                        }
                    }

                }
                if (IsDefenderAFriendlyInShieldFormation(attacker, defender))
                {
                    shouldCleave = true;
                }
            }

            return shouldCleave;
        }

        internal static bool CheckApplyCleave(Mission __instance, Agent attacker, Agent victim, MeleeCollisionReaction colReaction)
        {
            bool shouldCleave = false;

            if (attacker != null && victim != null)
            {
                if (attacker.IsHero)
                {
                    if (!CancelsDamageAndBlocksAttackBecauseOfNonEnemyCase(__instance, attacker, victim))
                    {
                        shouldCleave = true;
                    }
                }
                if (IsDefenderAFriendlyInShieldFormation(attacker, victim))
                {
                    shouldCleave = true;
                }
            }

            return shouldCleave;
        }

        internal static bool CancelsDamageAndBlocksAttackBecauseOfNonEnemyCase(Mission __instance, Agent attacker, Agent victim)
        {
            if (victim == null || attacker == null)
            {
                return false;
            }
            bool flag = !GameNetwork.IsSessionActive || (MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) <= 0 && MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) <= 0) || __instance.Mode == MissionMode.Duel || attacker.Controller == Agent.ControllerType.AI;
            bool flag2 = attacker.IsFriendOf(victim);
            return (flag && flag2) || (victim.IsHuman && !flag2 && !attacker.IsEnemyOf(victim));
        }
    }
}


