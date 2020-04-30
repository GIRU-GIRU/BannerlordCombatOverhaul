using HarmonyLib;
using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Engine;
using static HarmonyLib.AccessTools;
using GCO.Patches;
using System.Reflection;

namespace GCO.ReversePatches
{

    [HarmonyPatch]
    internal static class MissionReversePatches
    {
        //[HarmonyReversePatch]
        //[HarmonyPatch(typeof(Mission), "GetAttackCollisionResults")]
        //public static void GetAttackCollisionResults(float armorAmountFloat, WeaponComponentData shieldOnBack, AgentFlag victimAgentFlag, float victimAgentAbsorbedDamageRatio, float damageMultiplierOfBone, float combatDifficultyMultiplier, MissionWeapon victimShield, bool canGiveDamageToAgentShield, bool isVictimAgentLeftStance, bool isFriendlyFire, bool doesAttackerHaveMountAgent, bool doesVictimHaveMountAgent, Vec2 attackerAgentMovementVelocity, Vec3 attackerAgentMountMovementDirection, float attackerMovementDirectionAsAngle, Vec2 victimAgentMovementVelocity, Vec3 victimAgentMountMovementDirection, float victimMovementDirectionAsAngle, bool isVictimAgentSameWithAttackerAgent, bool isAttackerAgentMine, bool isAttackerAgentHasRiderAgent, bool isAttackerAgentRiderAgentIsMine, bool isAttackerAgentMount, bool isVictimAgentMine, bool isVictimAgentHasRiderAgent, bool isVictimAgentRiderAgentIsMine, bool isVictimAgentMount, bool isAttackerAgentNull, bool isAttackerAIControlled, BasicCharacterObject attackerAgentCharacter, BasicCharacterObject victimAgentCharacter, Vec3 attackerAgentMovementDirection, Vec3 attackerAgentVelocity, float attackerAgentMountChargeDamageProperty, Vec3 attackerAgentCurrentWeaponOffset, bool isAttackerAgentHuman, bool isAttackerAgentActive, bool isAttackerAgentCharging, bool isVictimAgentNull, float victimAgentScale, float victimAgentWeight, float victimAgentTotalEncumbrance, bool isVictimAgentHuman, Vec3 victimAgentVelocity, Vec3 victimAgentPosition, int weaponAttachBoneIndex, MissionWeapon offHandItem, bool isHeadShot, bool crushedThrough, bool IsVictimRiderAgentSameAsAttackerAgent, float momentumRemaining, ref AttackCollisionData attackCollisionData, bool cancelDamage, string victimAgentName, out CombatLogData combatLog)
        //{
        //    throw new NotImplementedException("Need to patch first");
        //}

        //[HarmonyReversePatch]  
        //[HarmonyPatch(typeof(Mission), "GetAttackCollisionResults"
        //    ,
        //    new Type[] {
        //        typeof(Agent),
        //        typeof(Agent),
        //        typeof(GameEntity),
        //        typeof(float),
        //        typeof(AttackCollisionData),
        //        typeof(bool),
        //        typeof(bool),
        //        typeof(WeaponComponent)
        //    }, new ArgumentType[] {
        //        ArgumentType.Normal,
        //        ArgumentType.Normal,
        //        ArgumentType.Normal,
        //        ArgumentType.Normal,
        //        ArgumentType.Ref,
        //        ArgumentType.Normal,
        //        ArgumentType.Normal,
        //        ArgumentType.Out }
        //    )]
        //internal static void GetAttackCollisionResults(this Mission __instance, 
        //    Agent attacker, 
        //    Agent victim, 
        //    GameEntity hitObject, 
        //    float momentumRemaining,
        //    ref AttackCollisionData attackCollisionData, 
        //    bool crushedThrough, 
        //    bool cancelDamage, 
        //    out WeaponComponentData shieldOnBack)
        //{
        //    throw new NotImplementedException("Need to patch first");
        //}



        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Mission), "CreateMissileBlow")]
        internal static Blow CreateMissileBlow(this Mission __instance, Agent attackerAgent, ref AttackCollisionData collisionData, Mission.Missile missile, Vec3 missilePosition, Vec3 missileStartingPosition)
        {
            throw new NotImplementedException("Need to patch first");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Mission), "RegisterBlow")]
        internal static void RegisterBlow(this Mission __instance, Agent attacker, Agent p, GameEntity hitEntity, Blow b, ref AttackCollisionData collisionData)
        {
            throw new NotImplementedException("Need to patch first");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Mission), "CalculateAttachedLocalFrame")]
        internal static MatrixFrame CalculateAttachedLocalFrame(this Mission __instance, ref MatrixFrame attachGlobalFrame, AttackCollisionData collisionData, WeaponComponentData currentUsageItem, Agent victim, GameEntity hitEntity, Vec3 movementVelocity, Vec3 missileAngularVelocity, MatrixFrame affectedShieldGlobalFrame, bool shouldMissilePenetrate)
        {
            throw new NotImplementedException("Need to patch first");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Mission), "GetDamageMultiplierOfCombatDifficulty")]
        internal static float GetDamageMultiplierOfCombatDifficulty(this Mission __instance, Agent victimAgent)
        {
            throw new NotImplementedException("Need to patch first");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Mission), "GetDamageMultiplierOfCombatDifficulty")]
        internal static bool GetDamageMultiplierOfCombatDifficulty(this Mission __instance, ref AttackCollisionData collisionData, Agent attacker)
        {
            throw new NotImplementedException("Need to patch first");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Mission), "OnEntityHit")]
        internal static void OnEntityHit(this Mission __instance, GameEntity realHitEntity, Agent attacker, int inflictedDamage, DamageTypes damageType, Vec3 position, Vec3 swingDirection, int affectorWeaponKind, int currentUsageIndex)
        {
            throw new NotImplementedException("Need to patch first");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Mission), "ComputeRelativeSpeedDiffOfAgents")]
        internal static float ComputeRelativeSpeedDiffOfAgents(Agent agentA, Agent agentB)
        {
            throw new NotImplementedException("Need to patch first");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Mission), "SpeedGraphFunction")]
        internal static float SpeedGraphFunction(this Mission __instance, float progress, StrikeType strikeType, Agent.UsageDirection attackDir)
        {
            throw new NotImplementedException("Need to patch first");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Mission), "HitWithAnotherBone", new Type[] { typeof(AttackCollisionData), typeof(Agent) }, new ArgumentType[] { ArgumentType.Ref, ArgumentType.Normal })]
        internal static bool HitWithAnotherBone(this Mission __instance, ref AttackCollisionData collisionData, Agent attacker)
        {
            throw new NotImplementedException("Need to patch first");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Mission), "AddCombatLogSafe")]
        internal static void AddCombatLogSafe(this Mission __instance, Agent attackerAgent, Agent victimAgent, GameEntity hitEntity, CombatLogData combatLog)
        {
            throw new NotImplementedException("Need to patch first");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Mission), "SpeedGraphFunction")]
        internal static float SpeedGraphFunction(this Mission current, StrikeType strikeType, Agent.UsageDirection attackDirection)
        {
            throw new NotImplementedException("Need to patch first");
        }
    }

}

