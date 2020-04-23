//using HarmonyLib;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using TaleWorlds.Core;
//using TaleWorlds.Engine;
//using TaleWorlds.Library;
//using TaleWorlds.MountAndBlade;
//using static HarmonyLib.AccessTools;

//namespace GCO.Features.ModdedMissionLogic
//{

//    public static class ProjectileBalanceExtensionMethods
//    {




//        private static MethodInfo accessTools_RegisterBlow = AccessTools.Method(typeof(Mission), "RegisterBlow", new Type[] {
//                    typeof(Agent),
//                    typeof(Agent),
//                    typeof(GameEntity),
//                    typeof(Blow),
//                    typeof(AttackCollisionData).MakeByRefType() });

//        private static MethodInfo accessTools_GetAttackCollisionResults = AccessTools.Method(typeof(Mission), "GetAttackCollisionResults", new Type[] {
//                    typeof(Agent),
//                    typeof(Agent),
//                    typeof(GameEntity),
//                    typeof(float),
//                    typeof(AttackCollisionData).MakeByRefType(),
//                    typeof(bool),
//                    typeof(bool),
//                    typeof(WeaponComponentData).MakeByRefType() });


//        private static FieldRef<Mission, Dictionary<int, Mission.Missile>> accessTools_missiles = AccessTools.FieldRefAccess<Mission, Dictionary<int, Mission.Missile>>("_missiles");

//        private static MethodInfo accessTools_CalculateAttachedLocalFrame = AccessTools.Method(typeof(Mission), "CalculateAttachedLocalFrame", new Type[] {
//                    typeof(MatrixFrame).MakeByRefType(),
//                    typeof(AttackCollisionData),
//                    typeof(WeaponComponentData),
//                    typeof(Agent),
//                    typeof(GameEntity),
//                    typeof(Vec3),
//                    typeof(Vec3),
//                    typeof(MatrixFrame),
//                    typeof(bool)});
//        internal static Dictionary<int, Mission.Missile> Get_missiles(ref Mission __instance)
//        {
//            return accessTools_missiles(__instance);
//        }

//        internal static void GetAttackCollisionResults(ref Mission __instance, Agent attacker, Agent victim, GameEntity hitObject, float momentumRemaining, ref AttackCollisionData attackCollisionData, bool crushedThrough, bool cancelDamage, out WeaponComponentData shieldOnBack)
//        {
//            shieldOnBack = null;
//            var obj = new object[] { attacker, victim, hitObject, momentumRemaining, attackCollisionData, crushedThrough, cancelDamage, shieldOnBack };

//            accessTools_GetAttackCollisionResults.Invoke(__instance, obj);
//            attackCollisionData = (AttackCollisionData)obj[4];
//        }

//        internal static Blow CreateMissileBlow(Agent attackerAgent, ref AttackCollisionData collisionData, Mission.Missile missile, Vec3 missilePosition, Vec3 missileStartingPosition)
//        {
//            Blow blow = new Blow(attackerAgent.Index);
//            blow.BlowFlag = BlowFlags.None;
//            blow.Direction = collisionData.MissileVelocity.NormalizedCopy();
//            blow.SwingDirection = blow.Direction;
//            blow.Position = collisionData.CollisionGlobalPosition;
//            blow.BoneIndex = collisionData.CollisionBoneIndex;
//            blow.MissileRecord.IsValid = true;
//            blow.MissileRecord.CurrentPosition = missilePosition;
//            blow.MissileRecord.StartingPosition = missileStartingPosition;
//            blow.MissileRecord.MissileItemKind = collisionData.AffectorWeaponKind;
//            blow.MissileRecord.ItemFlags = missile.Weapon.PrimaryItem.ItemFlags;
//            blow.MissileRecord.WeaponFlags = missile.Weapon.CurrentUsageItem.WeaponFlags;
//            blow.MissileRecord.Velocity = collisionData.MissileVelocity;
//            blow.StrikeType = (StrikeType)collisionData.StrikeType;
//            blow.DamageType = (DamageTypes)collisionData.DamageType;
//            blow.VictimBodyPart = collisionData.VictimHitBodyPart;
//            blow.WeaponRecord.FillWith(missile.Weapon.CurrentUsageItem, attackerAgent.Monster.GetBoneToAttachForItem(missile.Weapon.PrimaryItem), collisionData.CurrentUsageIndex);
//            blow.BaseMagnitude = collisionData.BaseMagnitude;
//            blow.MovementSpeedDamageModifier = collisionData.MovementSpeedDamageModifier;
//            blow.AbsorbedByArmor = (float)collisionData.AbsorbedByArmor;
//            blow.InflictedDamage = collisionData.InflictedDamage;
//            blow.SelfInflictedDamage = collisionData.SelfInflictedDamage;
//            blow.DamageCalculated = true;
//            return blow;
//        }

//        internal static void RegisterBlow(ref Mission __instance, Agent attacker, Agent p, GameEntity hitEntity, Blow b, ref AttackCollisionData collisionData)
//        {
//            var obj = new object[] { attacker, p, hitEntity, b, collisionData };

//            accessTools_RegisterBlow.Invoke(__instance, obj);
//            collisionData = (AttackCollisionData)obj[4];
//        }

//        internal static MatrixFrame CalculateAttachedLocalFrame(ref Mission __instance, ref MatrixFrame attachGlobalFrame, AttackCollisionData collisionData, WeaponComponentData currentUsageItem, Agent victim, GameEntity hitEntity, Vec3 movementVelocity, Vec3 missileAngularVelocity, MatrixFrame affectedShieldGlobalFrame, bool shouldMissilePenetrate)
//        {

//            return (MatrixFrame)accessTools_CalculateAttachedLocalFrame.Invoke(__instance, new object[] { attachGlobalFrame, collisionData,
//                    currentUsageItem, victim, hitEntity, movementVelocity, missileAngularVelocity, affectedShieldGlobalFrame, shouldMissilePenetrate });
//        }

//    }
//}
