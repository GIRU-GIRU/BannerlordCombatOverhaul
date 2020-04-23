using HarmonyLib;
using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using System.Collections.Generic;
using TaleWorlds.Engine;
using System.Reflection;
using static HarmonyLib.AccessTools;

namespace GCO.ReversePatches
{
    // TODO: Rework with Harmony Reverse Patches
    internal static class MissionReversePatches
    {
        //ensure you pass in the __instance
        internal static float GetDamageMultiplierOfCombatDifficulty(this Mission __instance, Agent victimAgent)
        {
            return Traverse.Create(__instance).Method("GetDamageMultiplierOfCombatDifficulty", new Type[] { typeof(Agent) }).GetValue<float>(victimAgent);
        }

        //ensure you pass in the __instance
        internal static bool HitWithAnotherBone(this Mission __instance, ref AttackCollisionData collisionData, Agent attacker)
        {
            return Traverse.Create(__instance).Method("HitWithAnotherBone", new Type[] { typeof(AttackCollisionData).MakeByRefType(), typeof(Agent) }).GetValue<bool>(collisionData, attacker);
        }

        //void method still requires a .GetValue
        internal static void OnEntityHit(this Mission __instance, GameEntity realHitEntity, Agent attacker, int inflictedDamage, DamageTypes damageType, Vec3 position, Vec3 swingDirection, int affectorWeaponKind, int currentUsageIndex)
        {
            Traverse.Create(__instance).Method("OnEntityHit", new Type[] {
                typeof(GameEntity),
                typeof(Agent),
                typeof(int),
                typeof(DamageTypes),
                typeof(Vec3),
                typeof(Vec3),
                typeof(int),
                typeof(int)
            }).GetValue(realHitEntity, attacker, inflictedDamage, damageType, position, swingDirection, affectorWeaponKind, currentUsageIndex);
        }

        internal static MethodInfo RegisterBlow = Method(typeof(Mission), "RegisterBlow", new Type[] {
                    typeof(Agent),
                    typeof(Agent),
                    typeof(GameEntity),
                    typeof(Blow),
                    typeof(AttackCollisionData).MakeByRefType() });

        internal static MethodInfo GetAttackCollisionResults = Method(typeof(Mission), "GetAttackCollisionResults", new Type[] {
                    typeof(Agent),
                    typeof(Agent),
                    typeof(GameEntity),
                    typeof(float),
                    typeof(AttackCollisionData).MakeByRefType(),
                    typeof(bool),
                    typeof(bool),
                    typeof(WeaponComponentData).MakeByRefType() });


        internal static FieldRef<Mission, Dictionary<int, Mission.Missile>> accessTools_missiles = AccessTools.FieldRefAccess<Mission, Dictionary<int, Mission.Missile>>("_missiles");

        internal static MethodInfo CalculateAttachedLocalFrame = Method(typeof(Mission), "CalculateAttachedLocalFrame", new Type[] {
                    typeof(MatrixFrame).MakeByRefType(),
                    typeof(AttackCollisionData),
                    typeof(WeaponComponentData),
                    typeof(Agent),
                    typeof(GameEntity),
                    typeof(Vec3),
                    typeof(Vec3),
                    typeof(MatrixFrame),
                    typeof(bool)});

        internal static Dictionary<int, Mission.Missile> GetMissiles(ref Mission __instance)
        {
            return accessTools_missiles(__instance);
        }
    }
}
