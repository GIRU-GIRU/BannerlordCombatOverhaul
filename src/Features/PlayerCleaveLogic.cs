using GCO.ModOptions;
using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace GCO.Features
{
    internal static class PlayerCleaveLogic
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
            float maxAngle = (float)(Math.PI / 180) * Config.ConfigSettings.AdditionalCleaveForTroopsInShieldWallAngleRestriction;
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


