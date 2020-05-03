using HarmonyLib;
using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using static HarmonyLib.AccessTools;

namespace GCO.Features.ModdedMissionLogic
{
    public static class ReverseExtensions
    {
        private static FieldRef<Mission, Dictionary<int, Mission.Missile>> accessTools_missiles = AccessTools.FieldRefAccess<Mission, Dictionary<int, Mission.Missile>>("_missiles");
        internal static Dictionary<int, Mission.Missile> Get_missiles(ref Mission __instance)
        {
            return accessTools_missiles(__instance);
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Mission), "GetAttackCollisionResults")]
        internal static void GetAttackCollisionResults(this Mission __instance, Agent attacker, Agent victim, GameEntity hitObject, float momentumRemaining, ref AttackCollisionData attackCollisionData, bool crushedThrough, bool cancelDamage, out WeaponComponentData shieldOnBack)
        {
            throw new NotImplementedException("Need to patch first");
        }

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
        internal static float ComputeRelativeSpeedDiffOfAgents(this Mission __instance, Agent agentA, Agent agentB)
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
        [HarmonyPatch(typeof(Mission), "HitWithAnotherBone")]
        internal static bool HitWithAnotherBone(this Mission __instance, ref AttackCollisionData collisionData, Agent attacker)
        {
            throw new NotImplementedException("Need to patch first");
        }
    }
}
