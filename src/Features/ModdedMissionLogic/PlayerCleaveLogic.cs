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
    [HarmonyPatch(typeof(Mission))]
    internal static class PlayerCleaveLogic
    {
        // This is what happens in vanilla if you hit shield
        //     if (!collisionData.IsColliderAgent || registeredBlow.InflictedDamage <= 0)
        //{
        //	colReaction = MeleeCollisionReaction.Bounced;
        //	return;
        //}
        [HarmonyPrefix]
        [HarmonyPatch("DecideWeaponCollisionReaction")]
        private static bool DecideWeaponCollisionReactionPrefix(Mission __instance, Blow registeredBlow, ref AttackCollisionData collisionData, Agent attacker, Agent defender, bool isFatalHit, bool isShruggedOff, ref MeleeCollisionReaction colReaction)
        {
            if (Config.ConfigSettings.CleaveEnabled)
            {
                if (!collisionData.IsColliderAgent || registeredBlow.InflictedDamage <= 0 || PlayerCleaveLogicExtensionMethods.IsDefenderAFriendlyInShieldFormation(attacker, defender))
                {
                    //collisionData = PlayerCleaveLogicExtensionMethods.GetAttackCollisionDataWithNoShieldBlock(collisionData);
                    colReaction = MeleeCollisionReaction.ContinueChecking;
                    return true;
                }
            }
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch("DecideWeaponCollisionReaction")]
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

        [HarmonyPostfix]
        [HarmonyPatch("MeleeHitCallback")]
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
        // using method created for debug purposes to mod game, PogChamp
        internal static AttackCollisionData GetAttackCollisionDataWithNoShieldBlock(AttackCollisionData attackCollision)
        {
            return AttackCollisionData.GetAttackCollisionDataForDebugPurpose(false, false, attackCollision.IsAlternativeAttack, attackCollision.IsColliderAgent, attackCollision.CollidedWithShieldOnBack, attackCollision.IsMissile, attackCollision.MissileHasPhysics, attackCollision.EntityExists, attackCollision.ThrustTipHit, attackCollision.MissileGoneUnderWater, CombatCollisionResult.None, attackCollision.CurrentUsageIndex, attackCollision.AffectorWeaponKind, attackCollision.StrikeType, attackCollision.DamageType, attackCollision.CollisionBoneIndex, attackCollision.VictimHitBodyPart, attackCollision.AttackBoneIndex, attackCollision.AttackDirection, attackCollision.PhysicsMaterialIndex, attackCollision.CollisionHitResultFlags, attackCollision.AttackProgress, attackCollision.CollisionDistanceOnWeapon, attackCollision.AttackerStunPeriod, attackCollision.DefenderStunPeriod, attackCollision.CurrentWeaponTipSpeed, attackCollision.MissileTotalDamage, 0f, attackCollision.ChargeVelocity, attackCollision.FallSpeed, attackCollision.WeaponRotUp, attackCollision.WeaponBlowDir, attackCollision.CollisionGlobalPosition, attackCollision.MissileVelocity, attackCollision.MissileStartingPosition, attackCollision.VictimAgentCurVelocity, new Vec3());
        }
        internal static bool IsDefenderAFriendlyInShieldFormation(Agent attacker, Agent defender)
        {
            return defender.Formation != null
                && defender.Formation.ArrangementOrder == ArrangementOrder.ArrangementOrderShieldWall
                && ((attacker.Formation != null && attacker.Formation.ArrangementOrder == ArrangementOrder.ArrangementOrderShieldWall) || attacker.IsPlayerControlled) // for some reason, player is always considered to be in a Line formation
                && attacker.Team == defender.Team;
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
                        if (registeredBlow.DamageType != TaleWorlds.Core.DamageTypes.Blunt && !isShruggedOff)
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


