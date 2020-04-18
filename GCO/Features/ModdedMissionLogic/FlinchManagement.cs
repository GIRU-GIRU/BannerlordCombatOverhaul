using HarmonyLib;
using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;
using NetworkMessages.FromServer;
using TaleWorlds.MountAndBlade.Network;
using TaleWorlds.Engine;

namespace GCO.Features.ModdedMissionLogic
{
    [HarmonyPatch(typeof(Mission))]
    public static class FlinchManagement
    {

        #region CreateBlow (currently unused)
        ////original method required a Blow object, but you must return boolean for harmony lib
        //[HarmonyPrefix]
        //[HarmonyPatch("CreateBlow")]
        //private static bool CreateBlow(Mission __instance, ref Blow __result, Agent attackerAgent, Agent victimAgent, ref AttackCollisionData collisionData, CrushThroughState cts, Vec3 blowDir, Vec3 swingDir, bool cancelDamage)
        //{
        //    Blow blow = new Blow(attackerAgent.Index);
        //    blow.VictimBodyPart = collisionData.VictimHitBodyPart;
        //    if (collisionData.AttackBlockedWithShield)
        //    {
        //        __result = blow;
        //    }
        //    ItemObject itemFromWeaponKind = ItemObject.GetItemFromWeaponKind(collisionData.AffectorWeaponKind);
        //    WeaponComponentData w = (itemFromWeaponKind != null) ? itemFromWeaponKind.GetWeaponWithUsageIndex(collisionData.CurrentUsageIndex) : null;
        //    bool flag = __instance.HitWithAnotherBone(ref collisionData, attackerAgent);
        //    if (collisionData.IsAlternativeAttack)
        //    {
        //        blow.AttackType = ((itemFromWeaponKind != null) ? AgentAttackType.Bash : AgentAttackType.Kick);
        //    }
        //    else
        //    {
        //        blow.AttackType = AgentAttackType.Standard;
        //    }


        //    sbyte weaponAttachBoneIndex = (itemFromWeaponKind != null) ? attackerAgent.Monster.GetBoneToAttachForItem(itemFromWeaponKind) : ((sbyte)-1);
        //    blow.WeaponRecord.FillWith(w, weaponAttachBoneIndex, collisionData.CurrentUsageIndex);
        //    blow.StrikeType = (StrikeType)collisionData.StrikeType;
        //    blow.DamageType = ((itemFromWeaponKind != null && !flag && !collisionData.IsAlternativeAttack) ? ((DamageTypes)collisionData.DamageType) : DamageTypes.Blunt);
        //    blow.NoIgnore = collisionData.IsAlternativeAttack;
        //    blow.AttackerStunPeriod = collisionData.AttackerStunPeriod;
        //    blow.DefenderStunPeriod = collisionData.DefenderStunPeriod;
        //    blow.BlowFlag = BlowFlags.None;
        //    if (collisionData.IsHorseCharge || (collisionData.IsAlternativeAttack && !attackerAgent.IsHuman))
        //    {
        //        blow.BlowFlag |= BlowFlags.KnockBack;
        //    }
        //    if (cts != CrushThroughState.None)
        //    {
        //        blow.BlowFlag |= BlowFlags.CrushThrough;
        //    }
        //    if (collisionData.IsColliderAgent)
        //    {
        //        if (itemFromWeaponKind != null && victimAgent.Health - (float)collisionData.InflictedDamage >= 1f)
        //        {
        //            ManagedParametersEnum managedParameterEnum;
        //            if (blow.DamageType == DamageTypes.Cut)
        //            {
        //                managedParameterEnum = ManagedParametersEnum.DamageInterruptAttackThresholdCut;
        //            }
        //            else if (blow.DamageType == DamageTypes.Pierce)
        //            {
        //                managedParameterEnum = ManagedParametersEnum.DamageInterruptAttackThresholdPierce;
        //            }
        //            else
        //            {
        //                managedParameterEnum = ManagedParametersEnum.DamageInterruptAttackThresholdBlunt;
        //            }
        //            float managedParameter = ManagedParameters.Instance.GetManagedParameter(managedParameterEnum);
        //            if ((float)collisionData.InflictedDamage <= managedParameter)
        //            {
        //                blow.BlowFlag |= BlowFlags.ShrugOff;
        //            }
        //        }
        //        if (blow.StrikeType == StrikeType.Thrust && blow.WeaponRecord.WeaponFlags.HasAnyFlag(WeaponFlags.WideGrip) && victimAgent.GetAgentFlags().HasAnyFlag(AgentFlag.CanRear) && attackerAgent != null && attackerAgent.MountAgent == null && Vec3.DotProduct(swingDir, victimAgent.Frame.rotation.f) < -0.5f && victimAgent.MovementVelocity.y > 4f && (float)collisionData.InflictedDamage >= ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.MakesRearAttackDamageThreshold) * __instance.GetDamageMultiplierOfCombatDifficulty(victimAgent))
        //        {
        //            blow.BlowFlag |= BlowFlags.MakesRear;
        //        }
        //        if (blow.StrikeType == StrikeType.Thrust && !collisionData.ThrustTipHit)
        //        {
        //            blow.BlowFlag |= BlowFlags.NonTipThrust;
        //        }
        //    }
        //    blow.Position = collisionData.CollisionGlobalPosition;
        //    blow.BoneIndex = collisionData.CollisionBoneIndex;
        //    blow.Direction = blowDir;
        //    blow.SwingDirection = swingDir;
        //    blow.BaseMagnitude = collisionData.BaseMagnitude;
        //    blow.MovementSpeedDamageModifier = collisionData.MovementSpeedDamageModifier;
        //    blow.InflictedDamage = collisionData.InflictedDamage;
        //    blow.SelfInflictedDamage = collisionData.SelfInflictedDamage;
        //    blow.AbsorbedByArmor = (float)collisionData.AbsorbedByArmor;
        //    blow.DamageCalculated = true;
        //    if (cancelDamage)
        //    {
        //        blow.BaseMagnitude = 0f;
        //        blow.MovementSpeedDamageModifier = 0f;
        //        blow.InflictedDamage = 0;
        //        blow.SelfInflictedDamage = 0;
        //        blow.AbsorbedByArmor = 0f;
        //    }

        //    //modifying the return value through Harmony
        //    __result = blow;
        //    return false;
        //}

        #endregion CreateBlow (currently unused)

        [HarmonyPrefix]
        [HarmonyPatch("GetDefendCollisionResultsAux")]
        private static bool GetDefendCollisionResultsAux(Mission __instance, Agent attackerAgent, Agent defenderAgent,
            CombatCollisionResult collisionResult, int weaponKind, int currentUsageIndex, bool isAlternativeAttack,
            StrikeType strikeType, Agent.UsageDirection attackDirection, float currentAttackSpeed, float collisionDistanceOnWeapon,
            float attackProgress, bool attackIsParried, ref float defenderStunPeriod, ref float attackerStunPeriod, ref bool crushedThrough,
            ref bool chamber)
        {

            ItemObject itemFromWeaponKind = ItemObject.GetItemFromWeaponKind(weaponKind);
            WeaponComponentData weaponComponentData = (itemFromWeaponKind != null) ? itemFromWeaponKind.GetWeaponWithUsageIndex(currentUsageIndex) : null;
            EquipmentIndex wieldedItemIndex = defenderAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
            if (wieldedItemIndex == EquipmentIndex.None)
            {
                wieldedItemIndex = defenderAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
            }
            WeaponComponentData weaponComponentData2 = (wieldedItemIndex != EquipmentIndex.None) ? defenderAgent.Equipment[wieldedItemIndex].CurrentUsageItem : null;
            float num = 10f;
            attackerStunPeriod = ((strikeType == StrikeType.Thrust) ? ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunPeriodAttackerThrust) : ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunPeriodAttackerSwing));
            chamber = false;
            float num2 = 0f;
            if (weaponComponentData != null)
            {
                float z = attackerAgent.GetCurWeaponOffset().z;
                float num3 = weaponComponentData.GetRealWeaponLength() + z;
                num2 = MBMath.ClampFloat((0.2f + collisionDistanceOnWeapon) / num3, 0.1f, 0.98f);
                float exraLinearSpeed = __instance.ComputeRelativeSpeedDiffOfAgents(attackerAgent, defenderAgent);
                float num4;
                if (strikeType == StrikeType.Thrust)
                {
                    num4 = CombatStatCalculator.CalculateBaseBlowMagnitudeForThrust((float)itemFromWeaponKind.PrimaryWeapon.ThrustSpeed / 11.7647057f * __instance.SpeedGraphFunction(attackProgress, strikeType, attackDirection), itemFromWeaponKind.Weight, exraLinearSpeed);
                }
                else
                {
                    num4 = CombatStatCalculator.CalculateBaseBlowMagnitudeForSwing((float)itemFromWeaponKind.PrimaryWeapon.SwingSpeed / 4.5454545f * __instance.SpeedGraphFunction(attackProgress, strikeType, attackDirection), weaponComponentData.GetRealWeaponLength(), itemFromWeaponKind.Weight, weaponComponentData.Inertia, weaponComponentData.CenterOfMass, num2, exraLinearSpeed);
                }
                if (strikeType == StrikeType.Thrust)
                {
                    num4 *= 0.8f;
                }
                else if (attackDirection == Agent.UsageDirection.AttackUp)
                {
                    num4 *= 1.25f;
                }
                num += num4;
            }
            float num5 = 1f;
            defenderStunPeriod = num * ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunMomentumTransferFactor);
            if (weaponComponentData2 != null)
            {
                if (weaponComponentData2.IsShield)
                {
                    float managedParameter = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightOffsetShield);
                    num5 += managedParameter * weaponComponentData2.Item.Weight;
                }
                else
                {
                    num5 = 0.9f;
                    float managedParameter2 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightMultiplierWeaponWeight);
                    num5 += managedParameter2 * weaponComponentData2.Item.Weight;
                    ItemObject.ItemTypeEnum itemType = weaponComponentData2.Item.ItemType;
                    if (itemType == ItemObject.ItemTypeEnum.TwoHandedWeapon)
                    {
                        managedParameter2 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightBonusTwoHanded);
                    }
                    else if (itemType == ItemObject.ItemTypeEnum.Polearm)
                    {
                        num5 += ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightBonusPolearm);
                    }
                }
                if (collisionResult == CombatCollisionResult.Parried)
                {
                    attackerStunPeriod += 0.1f;
                    num5 += ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightBonusActiveBlocked);

                    if (Config.ConfigSettings.HyperArmorEnabled)
                    {
                        if (GCOToolbox.GCOCheckForPlayerAgent(defenderAgent)) GCOToolbox.CreateHyperArmorBuff(defenderAgent);
                    }
                }
                else if (collisionResult == CombatCollisionResult.Blocked)
                {
                    if (Config.ConfigSettings.HyperArmorEnabled)
                    {
                        if (GCOToolbox.GCOCheckForPlayerAgent(defenderAgent)) GCOToolbox.CreateHyperArmorBuff(defenderAgent);
                    }
                }
                else if (collisionResult == CombatCollisionResult.ChamberBlocked)
                {
                    attackerStunPeriod += 0.2f;
                    num5 += ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightBonusChamberBlocked);
                    chamber = true;
                }
            }
            if (!defenderAgent.GetIsLeftStance())
            {
                num5 += ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightBonusRightStance);
            }
            defenderStunPeriod /= num5;
            float managedParameter3 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunPeriodMax);
            attackerStunPeriod = Math.Min(attackerStunPeriod, managedParameter3);
            defenderStunPeriod = Math.Min(defenderStunPeriod, managedParameter3);
            crushedThrough = (num > 38f && defenderStunPeriod > managedParameter3 - 0.03f && num2 > 0.5f);
            MissionGameModels.Current.AgentApplyDamageModel.CalculateEffects(attackerAgent, ref crushedThrough);
            if (chamber)
            {
                crushedThrough = false;
            }

            return false;
        }

        //original method required was a void, but you must return boolean for harmony lib
        [HarmonyPrefix]
        [HarmonyPatch("RegisterBlow")]
        private static bool RegisterBlow(Mission __instance, Agent attacker, Agent victim, GameEntity realHitEntity, Blow b, ref AttackCollisionData collisionData)
        {
            b.VictimBodyPart = collisionData.VictimHitBodyPart;
            if (!collisionData.AttackBlockedWithShield)
            {
                Blow blow = attacker.CreateBlowFromBlowAsReflection(b);

                if (GCOToolbox.GCOCheckForPlayerAgent(victim))
                {
                    GCOToolbox.CheckForProjectileFlinch(ref b, ref blow, collisionData, victim);
                    GCOToolbox.CheckToAddHyperarmor(ref b, ref blow);
                }


                if (collisionData.IsColliderAgent)
                {
                    if (b.SelfInflictedDamage > 0 && attacker != null && attacker.IsFriendOf(victim))
                    {
                        if (victim.IsMount && attacker.MountAgent != null)
                        {
                            attacker.MountAgent.RegisterBlow(blow);
                        }
                        else
                        {
                            attacker.RegisterBlow(blow);
                        }
                    }
                    victim.RegisterBlow(b);
                }
                else if (collisionData.EntityExists)
                {
                    __instance.OnEntityHit(realHitEntity, attacker, b.InflictedDamage, (DamageTypes)collisionData.DamageType, b.Position, b.SwingDirection, collisionData.AffectorWeaponKind, collisionData.CurrentUsageIndex);
                    if (b.SelfInflictedDamage > 0)
                    {
                        attacker.RegisterBlow(blow);
                    }
                }
            }
            foreach (MissionBehaviour missionBehaviour in __instance.MissionBehaviours)
            {
                missionBehaviour.OnRegisterBlow(attacker, victim, realHitEntity, b, ref collisionData);
            }

            return false;
        }
    }

    internal static class GCOToolbox
    {
        internal static bool GCOCheckForPlayerAgent(Agent agent)
        {
            bool isPlayer = false;

            if (agent != null)
            {
                if (agent.IsPlayerControlled && agent.IsHuman && agent.IsHero)
                {
                    isPlayer = true;
                }
            }

            return isPlayer;
        }

        internal static void CheckToAddHyperarmor(ref Blow b, ref Blow blow)
        {
            if (Global.IsHyperArmorActive())
            {
                b.BlowFlag |= BlowFlags.ShrugOff;
                blow.BlowFlag |= BlowFlags.ShrugOff;
            }
        }
        internal static void CreateHyperArmorBuff(Agent defenderAgent)
        {
            if (defenderAgent.IsPlayerControlled)
            {
                Global.ApplyHyperArmor();
            }
        }

        internal static void CheckForProjectileFlinch(ref Blow b, ref Blow blow, AttackCollisionData collisionData, Agent victim)
        {
            if (victim != null && b.IsMissile())
            {
                if (collisionData.VictimHitBodyPart != BoneBodyPartType.Head && collisionData.VictimHitBodyPart != BoneBodyPartType.Neck)
                {
                    if (b.InflictedDamage < (victim.HealthLimit * 0.4))
                    {
                        b.BlowFlag |= BlowFlags.ShrugOff;
                        blow.BlowFlag |= BlowFlags.ShrugOff;
                        InformationManager.DisplayMessage(
                            new InformationMessage("Player hyperarmor prevented flinch!", Colors.White));
                    }
                }
            }
        }
    }


    //grabbing the values of required methods for RegisterBlow/CreateBlow through Harmony. This must be in a seperate class
    internal static class FlinchManagementExtensionMethods
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
                typeof(int),
                typeof(int)
            }).GetValue(realHitEntity, attacker, inflictedDamage, damageType, position, swingDirection, affectorWeaponKind, currentUsageIndex);
        }

        internal static float ComputeRelativeSpeedDiffOfAgents(this Mission __instance, Agent agentA, Agent agentB)
        {
            Vec3 v = Vec3.Zero;
            if (agentA.MountAgent != null)
            {
                v = agentA.MountAgent.MovementVelocity.y * agentA.MountAgent.GetMovementDirection();
            }
            else
            {
                v.AsVec2 = agentA.MovementVelocity;
                v.RotateAboutZ(agentA.MovementDirectionAsAngle);
            }
            Vec3 v2 = Vec3.Zero;
            if (agentB.MountAgent != null)
            {
                v2 = agentB.MountAgent.MovementVelocity.y * agentB.MountAgent.GetMovementDirection();
            }
            else
            {
                v2.AsVec2 = agentB.MovementVelocity;
                v2.RotateAboutZ(agentB.MovementDirectionAsAngle);
            }
            return (v - v2).Length;

        }

        internal static float SpeedGraphFunction(this Mission __instance, float progress, StrikeType strikeType, Agent.UsageDirection attackDir)
        {
            bool flag = strikeType == StrikeType.Thrust;
            bool flag2 = attackDir == Agent.UsageDirection.AttackUp;
            ManagedParametersEnum managedParameterEnum;
            ManagedParametersEnum managedParameterEnum2;
            ManagedParametersEnum managedParameterEnum3;
            ManagedParametersEnum managedParameterEnum4;
            if (flag)
            {
                managedParameterEnum = ManagedParametersEnum.ThrustCombatSpeedGraphZeroProgressValue;
                managedParameterEnum2 = ManagedParametersEnum.ThrustCombatSpeedGraphFirstMaximumPoint;
                managedParameterEnum3 = ManagedParametersEnum.ThrustCombatSpeedGraphSecondMaximumPoint;
                managedParameterEnum4 = ManagedParametersEnum.ThrustCombatSpeedGraphOneProgressValue;
            }
            else if (flag2)
            {
                managedParameterEnum = ManagedParametersEnum.OverSwingCombatSpeedGraphZeroProgressValue;
                managedParameterEnum2 = ManagedParametersEnum.OverSwingCombatSpeedGraphFirstMaximumPoint;
                managedParameterEnum3 = ManagedParametersEnum.OverSwingCombatSpeedGraphSecondMaximumPoint;
                managedParameterEnum4 = ManagedParametersEnum.OverSwingCombatSpeedGraphOneProgressValue;
            }
            else
            {
                managedParameterEnum = ManagedParametersEnum.SwingCombatSpeedGraphZeroProgressValue;
                managedParameterEnum2 = ManagedParametersEnum.SwingCombatSpeedGraphFirstMaximumPoint;
                managedParameterEnum3 = ManagedParametersEnum.SwingCombatSpeedGraphSecondMaximumPoint;
                managedParameterEnum4 = ManagedParametersEnum.SwingCombatSpeedGraphOneProgressValue;
            }
            float managedParameter = ManagedParameters.Instance.GetManagedParameter(managedParameterEnum);
            float managedParameter2 = ManagedParameters.Instance.GetManagedParameter(managedParameterEnum2);
            float managedParameter3 = ManagedParameters.Instance.GetManagedParameter(managedParameterEnum3);
            float managedParameter4 = ManagedParameters.Instance.GetManagedParameter(managedParameterEnum4);
            float result;
            if (progress < managedParameter2)
            {
                result = (1f - managedParameter) / managedParameter2 * progress + managedParameter;
            }
            else if (managedParameter3 < progress)
            {
                result = (managedParameter4 - 1f) / (1f - managedParameter3) * (progress - managedParameter3) + 1f;
            }
            else
            {
                result = 1f;
            }
            return result;
        }





        // This is how to grab a private variable with harmony 
        //internal static bool GetVariableNameHere(this Mission __instance)
        //{
        //    return Traverse.Create(__instance).Field<VariableType>("VariableNameHere").Value;
        //}
    }
}