using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Engine;
using GCO.ReversePatches;
using GCO.Features;
using GCO.CopiedLogic;
using GCO.ModOptions;

namespace GCO.Patches
{
    internal static class MissionPatches
    {
        internal static bool CreateBlowPrefix(Mission __instance, ref Blow __result, Agent attackerAgent, Agent victimAgent, ref AttackCollisionData collisionData, CrushThroughState cts, Vec3 blowDir, Vec3 swingDir, bool cancelDamage)
        {
            Blow blow = new Blow(attackerAgent.Index);
            blow.VictimBodyPart = collisionData.VictimHitBodyPart;
            if (collisionData.AttackBlockedWithShield)
            {
                __result = blow;
            }
            ItemObject itemFromWeaponKind = ItemObject.GetItemFromWeaponKind(collisionData.AffectorWeaponKind);
            WeaponComponentData w = (itemFromWeaponKind != null) ? itemFromWeaponKind.GetWeaponWithUsageIndex(collisionData.CurrentUsageIndex) : null;
            bool flag = __instance.HitWithAnotherBone(ref collisionData, attackerAgent);
            if (collisionData.IsAlternativeAttack)
            {
                blow.AttackType = ((itemFromWeaponKind != null) ? AgentAttackType.Bash : AgentAttackType.Kick);
            }
            else
            {
                blow.AttackType = AgentAttackType.Standard;
            }


            sbyte weaponAttachBoneIndex = (itemFromWeaponKind != null) ? attackerAgent.Monster.GetBoneToAttachForItem(itemFromWeaponKind) : ((sbyte)-1);
            blow.WeaponRecord.FillWith(w, weaponAttachBoneIndex, collisionData.CurrentUsageIndex);
            blow.StrikeType = (StrikeType)collisionData.StrikeType;
            blow.DamageType = ((itemFromWeaponKind != null && !flag && !collisionData.IsAlternativeAttack) ? ((DamageTypes)collisionData.DamageType) : DamageTypes.Blunt);
            blow.NoIgnore = collisionData.IsAlternativeAttack;
            blow.AttackerStunPeriod = collisionData.AttackerStunPeriod;
            //blow.DefenderStunPeriod = collisionData.DefenderStunPeriod;

            blow.DefenderStunPeriod = GCOToolbox.GCOGetStaticFlinchPeriod(attackerAgent, collisionData.DefenderStunPeriod);


            blow.BlowFlag = BlowFlags.None;
            if (collisionData.IsHorseCharge || (collisionData.IsAlternativeAttack && !attackerAgent.IsHuman))
            {
                blow.BlowFlag |= BlowFlags.KnockBack;
            }
            if (cts != CrushThroughState.None)
            {
                blow.BlowFlag |= BlowFlags.CrushThrough;
            }
            if (collisionData.IsColliderAgent)
            {
                if (itemFromWeaponKind != null && victimAgent.Health - (float)collisionData.InflictedDamage >= 1f)
                {
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
                }
                if (blow.StrikeType == StrikeType.Thrust && blow.WeaponRecord.WeaponFlags.HasAnyFlag(WeaponFlags.WideGrip) && victimAgent.GetAgentFlags().HasAnyFlag(AgentFlag.CanRear) && attackerAgent != null && attackerAgent.MountAgent == null && Vec3.DotProduct(swingDir, victimAgent.Frame.rotation.f) < -0.5f && victimAgent.MovementVelocity.y > 4f && (float)collisionData.InflictedDamage >= ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.MakesRearAttackDamageThreshold) * __instance.GetDamageMultiplierOfCombatDifficulty(victimAgent))
                {
                    blow.BlowFlag |= BlowFlags.MakesRear;
                }
                if (blow.StrikeType == StrikeType.Thrust && !collisionData.ThrustTipHit)
                {
                    blow.BlowFlag |= BlowFlags.NonTipThrust;
                }
            }
            blow.Position = collisionData.CollisionGlobalPosition;
            blow.BoneIndex = collisionData.CollisionBoneIndex;
            blow.Direction = blowDir;
            blow.SwingDirection = swingDir;
            blow.BaseMagnitude = collisionData.BaseMagnitude;
            blow.MovementSpeedDamageModifier = collisionData.MovementSpeedDamageModifier;
            blow.InflictedDamage = collisionData.InflictedDamage;
            blow.SelfInflictedDamage = collisionData.SelfInflictedDamage;
            blow.AbsorbedByArmor = (float)collisionData.AbsorbedByArmor;
            blow.DamageCalculated = true;
            if (cancelDamage)
            {
                blow.BaseMagnitude = 0f;
                blow.MovementSpeedDamageModifier = 0f;
                blow.InflictedDamage = 0;
                blow.SelfInflictedDamage = 0;
                blow.AbsorbedByArmor = 0f;
            }

            //modifying the return value through Harmony
            __result = blow;
            return false;
        }

        internal static bool GetDefendCollisionResultsAuxPrefix(Mission __instance, Agent attackerAgent, Agent defenderAgent,
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
        internal static bool RegisterBlowPrefix(Mission __instance, Agent attacker, Agent victim, GameEntity realHitEntity, Blow b, ref AttackCollisionData collisionData)
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

        internal static void DecideWeaponCollisionReactionPostfix(Mission __instance, Blow registeredBlow, ref AttackCollisionData collisionData, Agent attacker, Agent defender, bool isFatalHit, bool isShruggedOff, ref MeleeCollisionReaction colReaction)
        {
            if (!Config.CompatibilitySettings.XorbarexCleaveExists)
            {
                if (PlayerCleaveLogic.CheckApplyCleave(__instance, attacker, defender, registeredBlow, isShruggedOff))
                {
                    colReaction = MeleeCollisionReaction.SlicedThrough;
                }
            }
        }

        internal static bool CancelsDamageAndBlocksAttackBecauseOfNonEnemyCasePrefix(ref bool __result, Agent attacker, Agent victim)
        {
            if (attacker != Mission.Current.MainAgent)
            {
                bool canMurder = Config.ConfigSettings.MurderEnabled && Mission.Current.Mode == MissionMode.StartUp;
                bool canTK = Config.ConfigSettings.TrueFriendlyFireEnabled && Mission.Current.Mode != MissionMode.StartUp;

                if (canMurder || canTK)
                {
                    if (victim == null || attacker == null)
                    {
                        return false;
                    }
                    bool flag = !GameNetwork.IsSessionActive || (MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) <= 0 && MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) <= 0) || Mission.Current.Mode == MissionMode.Duel || attacker.Controller == Agent.ControllerType.AI;
                    bool flag2 = attacker.IsFriendOf(victim);
                    __result = (flag && flag2) || (victim.IsHuman && !flag2 && !attacker.IsEnemyOf(victim));
                }
            }

            __result = false;
            return false;
        }

        internal static void MeleeHitCallbackPostfix(Mission __instance, ref AttackCollisionData collisionData, Agent attacker, Agent victim, GameEntity realHitEntity, float momentumRemainingToComputeDamage, ref float inOutMomentumRemaining, ref MeleeCollisionReaction colReaction, CrushThroughState cts, Vec3 blowDir, Vec3 swingDir, bool crushedThroughWithoutAgentCollision)
        {
            if (!Config.CompatibilitySettings.XorbarexCleaveExists)
            {
                if (PlayerCleaveLogic.CheckApplyCleave(__instance, attacker, victim, colReaction))
                {
                    if (attacker.HasMount)
                    {
                        inOutMomentumRemaining = momentumRemainingToComputeDamage * 0.25f;
                    }
                    else if (PlayerCleaveLogic.IsDefenderAFriendlyInShieldFormation(attacker, victim))
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
}
