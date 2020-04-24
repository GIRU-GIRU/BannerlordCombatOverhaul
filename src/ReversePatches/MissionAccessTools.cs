using HarmonyLib;
using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using System.Collections.Generic;
using TaleWorlds.Engine;
using static HarmonyLib.AccessTools;
using GCO.Patches;
using System.Reflection;

namespace GCO.ReversePatches
{
    class MissionAccessTools
    {
        private static FieldRef<Mission, Dictionary<int, Mission.Missile>> accessTools_missiles = FieldRefAccess<Mission, Dictionary<int, Mission.Missile>>("_missiles");

        private static MethodInfo accessTools_GetAttackCollisionResults = AccessTools.Method(typeof(Mission), "GetAttackCollisionResults", new Type[] {
                    typeof(Agent),
                    typeof(Agent),
                    typeof(GameEntity),
                    typeof(float),
                    typeof(AttackCollisionData).MakeByRefType(),
                    typeof(bool),
                    typeof(bool),
                    typeof(WeaponComponentData).MakeByRefType() });

        internal static void GetAttackCollisionResults(ref Mission __instance, Agent attacker, Agent victim, GameEntity hitObject, float momentumRemaining, ref AttackCollisionData attackCollisionData, bool crushedThrough, bool cancelDamage, out WeaponComponentData shieldOnBack)
        {
            shieldOnBack = null;
            var obj = new object[] { attacker, victim, hitObject, momentumRemaining, attackCollisionData, crushedThrough, cancelDamage, shieldOnBack };

            accessTools_GetAttackCollisionResults.Invoke(__instance, obj);
            attackCollisionData = (AttackCollisionData)obj[4];
        }
        internal static Dictionary<int, Mission.Missile> Get_missiles(ref Mission __instance)
        {
            return accessTools_missiles(__instance);
        }
    }
}
