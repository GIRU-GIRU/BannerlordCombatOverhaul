using HarmonyLib;
using JetBrains.Annotations;
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
using static HarmonyLib.AccessTools;

namespace GCO.Features.ModdedMissionLogic
{
    class ProjectileBalanceLogic
    {
        private static bool MissileHitCallbackPrefix(ref bool __result, ref Mission __instance, out int hitParticleIndex, ref AttackCollisionData collisionData, int missileIndex, Vec3 missileStartingPosition, Vec3 missilePosition, Vec3 missileAngularVelocity, Vec3 movementVelocity, MatrixFrame attachGlobalFrame, MatrixFrame affectedShieldGlobalFrame, int numDamagedAgents, Agent attacker, Agent victim, GameEntity hitEntity)
        {
            var _missiles = ProjectileBalanceExtensionMethods.Get_missiles(ref __instance);

            Mission.Missile missile = _missiles[missileIndex];
            WeaponFlags weaponFlags = missile.Weapon.CurrentUsageItem.WeaponFlags;
            float num = 1f;
            WeaponComponentData weaponComponentData = null;
            if (collisionData.AttackBlockedWithShield && weaponFlags.HasAnyFlag(WeaponFlags.CanPenetrateShield))
            {
                ProjectileBalanceExtensionMethods.GetAttackCollisionResults(ref __instance, attacker, victim, hitEntity, num, ref collisionData, false, false, out weaponComponentData);
                EquipmentIndex wieldedItemIndex = victim.GetWieldedItemIndex(Agent.HandIndex.OffHand);
                if ((float)collisionData.InflictedDamage > ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.ShieldPenetrationOffset) + ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.ShieldPenetrationFactor) * (float)victim.Equipment[wieldedItemIndex].GetShieldArmorForCurrentUsage())
                {
                    AttackCollisionData.UpdateDataForShieldPenetration(ref collisionData);
                    num *= 0.4f + MBRandom.RandomFloat * 0.2f;
                }
            }
            hitParticleIndex = -1;
            Mission.MissileCollisionReaction missileCollisionReaction = Mission.MissileCollisionReaction.Invalid;
            bool flag = !GameNetwork.IsSessionActive;
            bool missileHasPhysics = collisionData.MissileHasPhysics;
            PhysicsMaterial fromIndex = PhysicsMaterial.GetFromIndex(collisionData.PhysicsMaterialIndex);
            int num1 = fromIndex.IsValid ? (int) fromIndex.GetFlags() : 0;
            bool flag2 = (weaponFlags & WeaponFlags.AmmoSticksWhenShot) > (WeaponFlags)0UL;
            bool flag3 = (num1 & 1) == 0;
            bool flag4 = (uint)(num1 & 8) > 0U;
            MissionObject missionObject = null;
            if (victim == null && hitEntity != null)
            {
                GameEntity gameEntity = hitEntity;
                do
                {
                    missionObject = gameEntity.GetFirstScriptOfType<MissionObject>();
                    gameEntity = gameEntity.Parent;
                }
                while (missionObject == null && gameEntity != null);
                hitEntity = ((missionObject != null) ? missionObject.GameEntity : null);
            }
            Mission.MissileCollisionReaction missileCollisionReaction2;
            if (flag4)
            {
                missileCollisionReaction2 = Mission.MissileCollisionReaction.PassThrough;
            }
            else if (weaponFlags.HasAnyFlag(WeaponFlags.Burning))
            {
                missileCollisionReaction2 = Mission.MissileCollisionReaction.BecomeInvisible;
            }
            else if (!flag3 || !flag2)
            {
                missileCollisionReaction2 = Mission.MissileCollisionReaction.BounceBack;
            }
            else
            {
                missileCollisionReaction2 = Mission.MissileCollisionReaction.Stick;
            }
            bool flag5 = false;
            if (collisionData.MissileGoneUnderWater)
            {
                missileCollisionReaction = Mission.MissileCollisionReaction.BecomeInvisible;
                hitParticleIndex = 0;
            }
            else if (victim == null)
            {
                if (hitEntity != null)
                {
                    ProjectileBalanceExtensionMethods.GetAttackCollisionResults(ref __instance, attacker, victim, hitEntity, num, ref collisionData, false, false, out weaponComponentData);
                    Blow b = ProjectileBalanceExtensionMethods.CreateMissileBlow(attacker, ref collisionData, missile, missilePosition, missileStartingPosition);
                    ProjectileBalanceExtensionMethods.RegisterBlow(ref __instance, attacker, null, hitEntity, b, ref collisionData);
                }
                missileCollisionReaction = missileCollisionReaction2;
                hitParticleIndex = 0;
            }
            else if (collisionData.AttackBlockedWithShield)
            {
                ProjectileBalanceExtensionMethods.GetAttackCollisionResults(ref __instance, attacker, victim, hitEntity, num, ref collisionData, false, false, out weaponComponentData);
                missileCollisionReaction = (collisionData.IsShieldBroken ? Mission.MissileCollisionReaction.BecomeInvisible : missileCollisionReaction2);
                hitParticleIndex = 0;
            }
            else
            {
                if (attacker != null && attacker.IsFriendOf(victim))
                {
                    if (!missileHasPhysics)
                    {
                        if (flag)
                        {
                            if (attacker.Controller == Agent.ControllerType.AI)
                            {
                                flag5 = true;
                            }
                        }
                        else if ((MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) <= 0 && MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) <= 0) || Mission.Current.Mode == MissionMode.Duel)
                        {
                            flag5 = true;
                        }
                    }
                }
                else if (victim.IsHuman && !attacker.IsEnemyOf(victim))
                {
                    flag5 = true;
                }
                else if (flag && attacker != null && attacker.Controller == Agent.ControllerType.AI && victim.RiderAgent != null && attacker.IsFriendOf(victim.RiderAgent))
                {
                    flag5 = true;
                }
                if (flag5)
                {
                    if (flag && attacker == Agent.Main && attacker.IsFriendOf(victim))
                    {
                        InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_you_hit_a_friendly_troop", null).ToString(), Color.ConvertStringToColor("#D65252FF")));
                    }
                    missileCollisionReaction = Mission.MissileCollisionReaction.BecomeInvisible;
                }
                else
                {
                    bool flag6 = (weaponFlags & WeaponFlags.MultiplePenetration) > (WeaponFlags)0UL;
                    ProjectileBalanceExtensionMethods.GetAttackCollisionResults(ref __instance, attacker, victim, null, num, ref collisionData, false, false, out weaponComponentData);
                    Blow blow = ProjectileBalanceExtensionMethods.CreateMissileBlow(attacker, ref collisionData, missile, missilePosition, missileStartingPosition);
                    if (!collisionData.CollidedWithShieldOnBack && flag6 && numDamagedAgents > 0)
                    {
                        blow.InflictedDamage /= numDamagedAgents;
                        blow.SelfInflictedDamage /= numDamagedAgents;
                    }
                    ManagedParametersEnum managedParameterEnum;
                    if (blow.DamageType == DamageTypes.Cut)
                    {
                        managedParameterEnum = ManagedParametersEnum.DamageInterruptAttackThresholdCut;
                    }
                    else if (blow.DamageType == DamageTypes.Pierce)
                    {
                        managedParameterEnum = ManagedParametersEnum.DamageInterruptAttackThresholdPierce;
                    }
                    else
                    {
                        managedParameterEnum = ManagedParametersEnum.DamageInterruptAttackThresholdBlunt;
                    }
                    float managedParameter = ManagedParameters.Instance.GetManagedParameter(managedParameterEnum);
                    if ((float)collisionData.InflictedDamage <= managedParameter)
                    {
                        blow.BlowFlag |= BlowFlags.ShrugOff;
                    }
                    if (victim.State == AgentState.Active)
                    {
                        ProjectileBalanceExtensionMethods.RegisterBlow(ref __instance, attacker, victim, null, blow, ref collisionData);
                    }
                    hitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_blood_sword_enter");
                    if (flag6 && numDamagedAgents < 3)
                    {
                        missileCollisionReaction = Mission.MissileCollisionReaction.PassThrough;
                    }
                    else
                    {
                        if (missileCollisionReaction2 == Mission.MissileCollisionReaction.Stick && !collisionData.CollidedWithShieldOnBack)
                        {
                            bool flag7 = __instance.CombatType == Mission.MissionCombatType.Combat;
                            if (flag7)
                            {
                                bool flag8 = victim.IsHuman && collisionData.VictimHitBodyPart == BoneBodyPartType.Head;
                                flag7 = (victim.State != AgentState.Active || !flag8);
                            }
                            if (flag7)
                            {
                                float managedParameter2 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.MissileMinimumDamageToStick);
                                float num2 = 2f * managedParameter2;
                                if (!GameNetwork.IsClientOrReplay && (float)blow.InflictedDamage < managedParameter2 && blow.AbsorbedByArmor > num2)
                                {
                                    missileCollisionReaction = Mission.MissileCollisionReaction.BounceBack;
                                }
                            }
                            else
                            {
                                missileCollisionReaction = Mission.MissileCollisionReaction.BecomeInvisible;
                            }
                        }
                    }
                }
            }
            if (collisionData.CollidedWithShieldOnBack && weaponComponentData != null && victim != null && victim.IsMainAgent)
            {
                InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_hit_shield_on_back", null).ToString(), Color.ConvertStringToColor("#FFFFFFFF")));
            }
            MatrixFrame attachLocalFrame;
            if (!collisionData.MissileHasPhysics && !collisionData.MissileGoneUnderWater)
            {
                bool shouldMissilePenetrate = missileCollisionReaction == Mission.MissileCollisionReaction.Stick;
                attachLocalFrame = ProjectileBalanceExtensionMethods.CalculateAttachedLocalFrame(ref __instance, ref attachGlobalFrame, collisionData, missile.Weapon.CurrentUsageItem, victim, hitEntity, movementVelocity, missileAngularVelocity, affectedShieldGlobalFrame, shouldMissilePenetrate);
            }
            else
            {
                attachLocalFrame = attachGlobalFrame;
                missionObject = null;
            }
            Vec3 zero = Vec3.Zero;
            Vec3 zero2 = Vec3.Zero;
            if (missileCollisionReaction == Mission.MissileCollisionReaction.BounceBack)
            {
                WeaponFlags weaponFlags2 = weaponFlags & WeaponFlags.AmmoBreakOnBounceBackMask;
                if ((weaponFlags2 == WeaponFlags.AmmoCanBreakOnBounceBack && collisionData.MissileVelocity.Length > ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BreakableProjectileMinimumBreakSpeed)) || weaponFlags2 == WeaponFlags.AmmoBreaksOnBounceBack)
                {
                    missileCollisionReaction = Mission.MissileCollisionReaction.BecomeInvisible;
                    hitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_broken_arrow");
                }
                else
                {
                    missile.CalculateBounceBackVelocity(missileAngularVelocity, collisionData, out zero, out zero2);
                }
            }
            __instance.HandleMissileCollisionReaction(missileIndex, missileCollisionReaction, attachLocalFrame, attacker, victim, collisionData.AttackBlockedWithShield, collisionData.CollisionBoneIndex, missionObject, zero, zero2, -1);
            foreach (MissionBehaviour missionBehaviour in __instance.MissionBehaviours)
            {
                missionBehaviour.OnMissileHit(attacker, victim, flag5);
            }
            __result = missileCollisionReaction != Mission.MissileCollisionReaction.PassThrough;

            return false;
        }

        public static class ProjectileBalanceExtensionMethods
        {
            private static MethodInfo accessTools_RegisterBlow = AccessTools.Method(typeof(Mission), "RegisterBlow", new Type[] {
                    typeof(Agent),
                    typeof(Agent),
                    typeof(GameEntity),
                    typeof(Blow),
                    typeof(AttackCollisionData).MakeByRefType() });

            private static MethodInfo accessTools_GetAttackCollisionResults = AccessTools.Method(typeof(Mission), "GetAttackCollisionResults", new Type[] {
                    typeof(Agent),
                    typeof(Agent),
                    typeof(GameEntity),
                    typeof(float),
                    typeof(AttackCollisionData).MakeByRefType(),
                    typeof(bool),
                    typeof(bool),
                    typeof(WeaponComponentData).MakeByRefType() });


            private static FieldRef<Mission, Dictionary<int, Mission.Missile>> accessTools_missiles = AccessTools.FieldRefAccess<Mission, Dictionary<int, Mission.Missile>>("_missiles");

            private static MethodInfo accessTools_CalculateAttachedLocalFrame = AccessTools.Method(typeof(Mission), "CalculateAttachedLocalFrame", new Type[] {
                    typeof(MatrixFrame).MakeByRefType(),
                    typeof(AttackCollisionData),
                    typeof(WeaponComponentData),
                    typeof(Agent),
                    typeof(GameEntity),
                    typeof(Vec3),
                    typeof(Vec3),
                    typeof(MatrixFrame),
                    typeof(bool)});
            internal static Dictionary<int, Mission.Missile> Get_missiles(ref Mission __instance)
            {
                return accessTools_missiles(__instance);
            }

            internal static void GetAttackCollisionResults(ref Mission __instance, Agent attacker, Agent victim, GameEntity hitObject, float momentumRemaining, ref AttackCollisionData attackCollisionData, bool crushedThrough, bool cancelDamage, out WeaponComponentData shieldOnBack)
            {
                shieldOnBack = null;
                var obj = new object[] { attacker, victim, hitObject, momentumRemaining, attackCollisionData, crushedThrough, cancelDamage, shieldOnBack };
                
                accessTools_GetAttackCollisionResults.Invoke(__instance, obj);
                attackCollisionData = (AttackCollisionData)obj[4];
            }

            internal static Blow CreateMissileBlow(Agent attackerAgent, ref AttackCollisionData collisionData, Mission.Missile missile, Vec3 missilePosition, Vec3 missileStartingPosition)
            {
                Blow blow = new Blow(attackerAgent.Index);
                blow.BlowFlag = BlowFlags.None;
                blow.Direction = collisionData.MissileVelocity.NormalizedCopy();
                blow.SwingDirection = blow.Direction;
                blow.Position = collisionData.CollisionGlobalPosition;
                blow.BoneIndex = collisionData.CollisionBoneIndex;
                blow.MissileRecord.IsValid = true;
                blow.MissileRecord.CurrentPosition = missilePosition;
                blow.MissileRecord.StartingPosition = missileStartingPosition;
                blow.MissileRecord.MissileItemKind = collisionData.AffectorWeaponKind;
                blow.MissileRecord.ItemFlags = missile.Weapon.PrimaryItem.ItemFlags;
                blow.MissileRecord.WeaponFlags = missile.Weapon.CurrentUsageItem.WeaponFlags;
                blow.MissileRecord.Velocity = collisionData.MissileVelocity;
                blow.StrikeType = (StrikeType)collisionData.StrikeType;
                blow.DamageType = (DamageTypes)collisionData.DamageType;
                blow.VictimBodyPart = collisionData.VictimHitBodyPart;
                blow.WeaponRecord.FillWith(missile.Weapon.CurrentUsageItem, attackerAgent.Monster.GetBoneToAttachForItem(missile.Weapon.PrimaryItem), collisionData.CurrentUsageIndex);
                blow.BaseMagnitude = collisionData.BaseMagnitude;
                blow.MovementSpeedDamageModifier = collisionData.MovementSpeedDamageModifier;
                blow.AbsorbedByArmor = (float)collisionData.AbsorbedByArmor;
                blow.InflictedDamage = collisionData.InflictedDamage;
                blow.SelfInflictedDamage = collisionData.SelfInflictedDamage;
                blow.DamageCalculated = true;
                return blow;
            }

            internal static void RegisterBlow(ref Mission __instance, Agent attacker, Agent p, GameEntity hitEntity, Blow b, ref AttackCollisionData collisionData)
            {
               var obj = new object[] { attacker, p, hitEntity, b, collisionData };

               accessTools_RegisterBlow.Invoke(__instance, obj);
               collisionData = (AttackCollisionData)obj[4];
            }

            internal static MatrixFrame CalculateAttachedLocalFrame(ref Mission __instance, ref MatrixFrame attachGlobalFrame, AttackCollisionData collisionData, WeaponComponentData currentUsageItem, Agent victim, GameEntity hitEntity, Vec3 movementVelocity, Vec3 missileAngularVelocity, MatrixFrame affectedShieldGlobalFrame, bool shouldMissilePenetrate)
            {
                
                return (MatrixFrame)accessTools_CalculateAttachedLocalFrame.Invoke(__instance, new object[] { attachGlobalFrame, collisionData, 
                    currentUsageItem, victim, hitEntity, movementVelocity, missileAngularVelocity, affectedShieldGlobalFrame, shouldMissilePenetrate });
            }
        }
    }
}
