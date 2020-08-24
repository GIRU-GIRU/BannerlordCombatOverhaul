using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using GCO.ReversePatches;
using System.Linq;
using System;
using GCO.GCOMissionLogic;
using GCO.ModOptions;
using TaleWorlds.DotNet;
using GCO.GCOToolbox;

namespace GCO.Patches
{
    class MissionPatchesProjectile
    {
        internal static void GetWeaponSkillPostfix(ref int __result, BasicCharacterObject character, WeaponComponentData equippedItem)
        {
            SkillObject skill = DefaultSkills.Athletics;
            if (equippedItem != null)
            {
                skill = equippedItem.RelevantSkill;
            }

            var skillAmount = character.GetSkillValue(skill);
            var amountToReturn = GCOToolbox.GCOToolbox.ProjectileBalance.IfCrossbowEmpowerStat(skillAmount, skill);

            __result = amountToReturn;
        }

        internal static bool MissileHitCallbackPrefix(ref bool __result, ref Mission __instance, out int hitParticleIndex, ref AttackCollisionData collisionData, int missileIndex, Vec3 missileStartingPosition, Vec3 missilePosition, Vec3 missileAngularVelocity, Vec3 movementVelocity, MatrixFrame attachGlobalFrame, MatrixFrame affectedShieldGlobalFrame, int numDamagedAgents, Agent attacker, Agent victim, GameEntity hitEntity)
        {
            var _missiles = MissionAccessTools.Get_missiles(ref __instance);
            Mission.Missile missile = _missiles[missileIndex];

            bool isHorseArcher = GCOToolbox.GCOToolbox.ProjectileBalance.CheckForHorseArcher(victim);
            bool makesRear = GCOToolbox.GCOToolbox.ProjectileBalance.ApplyHorseCrippleLogic(victim, collisionData.VictimHitBodyPart);

            WeaponFlags weaponFlags1 = missile.Weapon.CurrentUsageItem.WeaponFlags;
            float momentumRemaining = 1f;
            WeaponComponentData shieldOnBack = (WeaponComponentData)null;
            if (collisionData.AttackBlockedWithShield && weaponFlags1.HasAnyFlag<WeaponFlags>(WeaponFlags.CanPenetrateShield))
            {
                GetAttackCollisionResultsPrefix(ref __instance, isHorseArcher, missile, attacker, victim, hitEntity, momentumRemaining, ref collisionData, false, false, out shieldOnBack);
                EquipmentIndex wieldedItemIndex = victim.GetWieldedItemIndex(Agent.HandIndex.OffHand);
                if ((double)collisionData.InflictedDamage > (double)ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.ShieldPenetrationOffset) + (double)ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.ShieldPenetrationFactor) * (double)victim.Equipment[wieldedItemIndex].GetShieldArmorForCurrentUsage())
                {
                    AttackCollisionData.UpdateDataForShieldPenetration(ref collisionData);
                    momentumRemaining *= (float)(0.400000005960464 + (double)MBRandom.RandomFloat * 0.200000002980232);
                }
            }
            hitParticleIndex = -1;
            bool flag1 = !GameNetwork.IsSessionActive;
            bool missileHasPhysics = collisionData.MissileHasPhysics;
            PhysicsMaterial fromIndex = PhysicsMaterial.GetFromIndex(collisionData.PhysicsMaterialIndex);
            int num1 = fromIndex.IsValid ? (int)fromIndex.GetFlags() : 0;
            bool flag2 = (weaponFlags1 & WeaponFlags.AmmoSticksWhenShot) > (WeaponFlags)0;
            bool flag3 = (num1 & 1) == 0;
            bool flag4 = (uint)(num1 & 8) > 0U;
            MissionObject attachedMissionObject = (MissionObject)null;
            if (victim == null && (NativeObject)hitEntity != (NativeObject)null)
            {
                GameEntity gameEntity = hitEntity;
                do
                {
                    attachedMissionObject = gameEntity.GetFirstScriptOfType<MissionObject>();
                    gameEntity = gameEntity.Parent;
                }
                while (attachedMissionObject == null && (NativeObject)gameEntity != (NativeObject)null);
                hitEntity = attachedMissionObject?.GameEntity;
            }
            Mission.MissileCollisionReaction collisionReaction1 = !flag4 ? (!weaponFlags1.HasAnyFlag<WeaponFlags>(WeaponFlags.Burning) ? (!flag3 || !flag2 ? Mission.MissileCollisionReaction.BounceBack : Mission.MissileCollisionReaction.Stick) : Mission.MissileCollisionReaction.BecomeInvisible) : Mission.MissileCollisionReaction.PassThrough;
            bool isCanceled = false;
            Mission.MissileCollisionReaction collisionReaction2;
            if (collisionData.MissileGoneUnderWater)
            {
                collisionReaction2 = Mission.MissileCollisionReaction.BecomeInvisible;
                hitParticleIndex = 0;
            }
            else if (victim == null)
            {
                if ((NativeObject)hitEntity != (NativeObject)null)
                {
                    GetAttackCollisionResultsPrefix(ref __instance, isHorseArcher, missile, attacker, victim, hitEntity, momentumRemaining, ref collisionData, false, false, out shieldOnBack);
                    Blow missileBlow = __instance.CreateMissileBlow(attacker, ref collisionData, missile, missilePosition, missileStartingPosition);
                    __instance.RegisterBlow(attacker, (Agent)null, hitEntity, missileBlow, ref collisionData);
                }
                collisionReaction2 = collisionReaction1;
                hitParticleIndex = 0;
            }
            else if (collisionData.AttackBlockedWithShield)
            {
                GetAttackCollisionResultsPrefix(ref __instance, isHorseArcher, missile, attacker, victim, hitEntity, momentumRemaining, ref collisionData, false, false, out shieldOnBack);
                collisionReaction2 = collisionData.IsShieldBroken ? Mission.MissileCollisionReaction.BecomeInvisible : collisionReaction1;
                hitParticleIndex = 0;
            }
            else
            {
                if (attacker != null && attacker.IsFriendOf(victim))
                {
                    if (!missileHasPhysics)
                    {
                        if (flag1)
                        {
                            if (attacker.Controller == Agent.ControllerType.AI)
                                isCanceled = true;
                        }
                        else if (MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) <= 0 && MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) <= 0 || __instance.Mode == MissionMode.Duel)
                            isCanceled = true;
                    }
                }
                else if (victim.IsHuman && !attacker.IsEnemyOf(victim))
                    isCanceled = true;
                else if (flag1 && attacker != null && (attacker.Controller == Agent.ControllerType.AI && victim.RiderAgent != null) && attacker.IsFriendOf(victim.RiderAgent))
                    isCanceled = true;
                if (isCanceled)
                {
                    if (flag1 && attacker == Agent.Main && attacker.IsFriendOf(victim))
                        InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_you_hit_a_friendly_troop", (string)null).ToString(), Color.ConvertStringToColor("#D65252FF")));
                    collisionReaction2 = Mission.MissileCollisionReaction.BecomeInvisible;
                }
                else
                {
                    bool flag5 = (weaponFlags1 & WeaponFlags.MultiplePenetration) > (WeaponFlags)0;
                    GetAttackCollisionResultsPrefix(ref __instance, isHorseArcher, missile, attacker, victim, (GameEntity)null, momentumRemaining, ref collisionData, false, false, out shieldOnBack);
                    Blow missileBlow = __instance.CreateMissileBlow(attacker, ref collisionData, missile, missilePosition, missileStartingPosition);
                    if (makesRear) missileBlow.BlowFlag = BlowFlags.MakesRear;

                    if (!collisionData.CollidedWithShieldOnBack & flag5 && numDamagedAgents > 0)
                    {
                        missileBlow.InflictedDamage /= numDamagedAgents;
                        missileBlow.SelfInflictedDamage /= numDamagedAgents;
                    }
                    float managedParameter1 = ManagedParameters.Instance.GetManagedParameter(missileBlow.DamageType != DamageTypes.Cut ? (missileBlow.DamageType != DamageTypes.Pierce ? ManagedParametersEnum.DamageInterruptAttackThresholdBlunt : ManagedParametersEnum.DamageInterruptAttackThresholdPierce) : ManagedParametersEnum.DamageInterruptAttackThresholdCut);
                    if ((double)collisionData.InflictedDamage <= (double)managedParameter1)
                        missileBlow.BlowFlag |= BlowFlags.ShrugOff;
                    if (victim.State == AgentState.Active)
                        __instance.RegisterBlow(attacker, victim, (GameEntity)null, missileBlow, ref collisionData);
                    hitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_blood_sword_enter");
                    if (flag5 && numDamagedAgents < 3)
                    {
                        collisionReaction2 = Mission.MissileCollisionReaction.PassThrough;
                    }
                    else
                    {
                        collisionReaction2 = collisionReaction1;
                        if (collisionReaction1 == Mission.MissileCollisionReaction.Stick && !collisionData.CollidedWithShieldOnBack)
                        {
                            bool flag6 = __instance.CombatType == Mission.MissionCombatType.Combat;
                            if (flag6)
                            {
                                bool flag7 = victim.IsHuman && collisionData.VictimHitBodyPart == BoneBodyPartType.Head;
                                flag6 = victim.State != AgentState.Active || !flag7;
                            }
                            if (flag6)
                            {
                                float managedParameter2 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.MissileMinimumDamageToStick);
                                float num2 = 2f * managedParameter2;
                                if (!GameNetwork.IsClientOrReplay && (double)missileBlow.InflictedDamage < (double)managedParameter2 && (double)missileBlow.AbsorbedByArmor > (double)num2)
                                    collisionReaction2 = Mission.MissileCollisionReaction.BounceBack;
                            }
                            else
                                collisionReaction2 = Mission.MissileCollisionReaction.BecomeInvisible;
                        }
                    }
                }
            }
            if (collisionData.CollidedWithShieldOnBack && shieldOnBack != null && (victim != null && victim.IsMainAgent))
                InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_hit_shield_on_back", (string)null).ToString(), Color.ConvertStringToColor("#FFFFFFFF")));
            MatrixFrame attachLocalFrame;
            if (!collisionData.MissileHasPhysics && !collisionData.MissileGoneUnderWater)
            {
                bool shouldMissilePenetrate = collisionReaction2 == Mission.MissileCollisionReaction.Stick;
                attachLocalFrame = __instance.CalculateAttachedLocalFrame(ref attachGlobalFrame, collisionData, missile.Weapon.CurrentUsageItem, victim, hitEntity, movementVelocity, missileAngularVelocity, affectedShieldGlobalFrame, shouldMissilePenetrate);
            }
            else
            {
                attachLocalFrame = attachGlobalFrame;
                attachedMissionObject = (MissionObject)null;
            }
            Vec3 velocity = Vec3.Zero;
            Vec3 angularVelocity = Vec3.Zero;
            if (collisionReaction2 == Mission.MissileCollisionReaction.BounceBack)
            {
                WeaponFlags weaponFlags2 = weaponFlags1 & WeaponFlags.AmmoBreakOnBounceBackMask;
                if (weaponFlags2 == WeaponFlags.AmmoCanBreakOnBounceBack && (double)collisionData.MissileVelocity.Length > (double)ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BreakableProjectileMinimumBreakSpeed) || weaponFlags2 == WeaponFlags.AmmoBreaksOnBounceBack)
                {
                    collisionReaction2 = Mission.MissileCollisionReaction.BecomeInvisible;
                    hitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_broken_arrow");
                }
                else
                    missile.CalculateBounceBackVelocity(missileAngularVelocity, collisionData, out velocity, out angularVelocity);
            }
            __instance.HandleMissileCollisionReaction(missileIndex, collisionReaction2, attachLocalFrame, attacker, victim, collisionData.AttackBlockedWithShield, collisionData.CollisionBoneIndex, attachedMissionObject, velocity, angularVelocity, -1);
            foreach (MissionBehaviour missionBehaviour in __instance.MissionBehaviours)
                missionBehaviour.OnMissileHit(attacker, victim, isCanceled);
            __result = collisionReaction2 != Mission.MissileCollisionReaction.PassThrough;

            return false;
        }

        internal static bool GetAttackCollisionResultsPrefix(ref Mission __instance, bool isHorseArcher, Mission.Missile missile, Agent attackerAgent, Agent victimAgent, GameEntity hitObject, float momentumRemaining, ref AttackCollisionData attackCollisionData, bool crushedThrough, bool cancelDamage, out WeaponComponentData shieldOnBack)
        {
            bool flag = attackerAgent == null;
            bool flag2 = victimAgent == null;
            float armorAmountFloat = 0f;
            if (!flag2)
            {
                armorAmountFloat = victimAgent.GetBaseArmorEffectivenessForBodyPart(attackCollisionData.VictimHitBodyPart);
            }
            shieldOnBack = null;
            if (!flag2 && (victimAgent.GetAgentFlags() & AgentFlag.CanWieldWeapon) != AgentFlag.None)
            {
                EquipmentIndex wieldedItemIndex = victimAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
                victimAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
                for (int i = 0; i < 4; i++)
                {
                    WeaponComponentData currentUsageItem = victimAgent.Equipment[i].CurrentUsageItem;
                    if (i != (int)wieldedItemIndex && currentUsageItem != null && currentUsageItem.IsShield)
                    {
                        shieldOnBack = currentUsageItem;
                        break;
                    }
                }
            }
            MissionWeapon victimShield = default(MissionWeapon);
            if (!flag2 && (victimAgent.GetAgentFlags() & AgentFlag.CanWieldWeapon) != AgentFlag.None)
            {
                EquipmentIndex wieldedItemIndex2 = victimAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
                if (wieldedItemIndex2 != EquipmentIndex.None)
                {
                    victimShield = victimAgent.Equipment[wieldedItemIndex2];
                }
            }
            Vec3 attackerAgentMountMovementDirection = default(Vec3);
            if (!flag && attackerAgent.HasMount)
            {
                attackerAgentMountMovementDirection = attackerAgent.MountAgent.GetMovementDirection();
            }
            Vec3 victimAgentMountMovementDirection = default(Vec3);
            if (!flag2 && victimAgent.HasMount)
            {
                victimAgentMountMovementDirection = victimAgent.MountAgent.GetMovementDirection();
            }
            bool flag3 = attackerAgent == victimAgent;
            ItemObject itemFromWeaponKind = ItemObject.GetItemFromWeaponKind(attackCollisionData.AffectorWeaponKind);
            int weaponAttachBoneIndex = (int)((itemFromWeaponKind != null && !flag && attackerAgent.IsHuman) ? attackerAgent.Monster.GetBoneToAttachForItem(itemFromWeaponKind) : -1);
            DestructableComponent destructableComponent = (hitObject != null) ? hitObject.GetFirstScriptOfTypeInFamily<DestructableComponent>() : null;
            bool flag4;
            if (!flag3 && (flag2 || !victimAgent.IsFriendOf(attackerAgent)))
            {
                if (destructableComponent != null)
                {
                    BattleSideEnum battleSide = destructableComponent.BattleSide;
                    Team team = attackerAgent.Team;
                    BattleSideEnum? battleSideEnum = (team != null) ? new BattleSideEnum?(team.Side) : null;
                    flag4 = (battleSide == battleSideEnum.GetValueOrDefault() & battleSideEnum != null);
                }
                else
                {
                    flag4 = false;
                }
            }
            else
            {
                flag4 = true;
            }
            bool isFriendlyFire = flag4;
            MissionWeapon offHandItem = default(MissionWeapon);
            if (!flag && (attackerAgent.GetAgentFlags() & AgentFlag.CanWieldWeapon) != AgentFlag.None)
            {
                EquipmentIndex wieldedItemIndex3 = attackerAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
                if (wieldedItemIndex3 != EquipmentIndex.None)
                {
                    offHandItem = attackerAgent.Equipment[wieldedItemIndex3];
                }
            }
            bool isHeadShot = attackCollisionData.VictimHitBodyPart == BoneBodyPartType.Head;
            float victimAgentAbsorbedDamageRatio = 0f;
            float num = 0f;
            float victimMovementDirectionAsAngle = 0f;
            float victimAgentScale = 0f;
            float victimAgentWeight = 0f;
            float victimAgentTotalEncumbrance = 0f;
            float combatDifficultyMultiplier = 1f;
            AgentFlag victimAgentFlag = AgentFlag.CanAttack;
            bool isVictimAgentLeftStance = false;
            bool doesVictimHaveMountAgent = false;
            bool isVictimAgentMine = false;
            bool flag5 = false;
            bool isVictimAgentRiderAgentIsMine = false;
            bool isVictimAgentMount = false;
            bool isVictimAgentHuman = false;
            bool isVictimRiderAgentSameAsAttackerAgent = false;
            BasicCharacterObject victimAgentCharacter = new BasicCharacterObject();
            Vec2 victimAgentMovementVelocity = default(Vec2);
            Vec3 victimAgentPosition = default(Vec3);
            Vec3 victimAgentVelocity = default(Vec3);
            string victimAgentName = string.Empty;
            if (!flag2)
            {
                isVictimAgentMount = victimAgent.IsMount;
                isVictimAgentMine = victimAgent.IsMine;
                bool isMainAgent = victimAgent.IsMainAgent;
                isVictimAgentHuman = victimAgent.IsHuman;
                isVictimAgentLeftStance = victimAgent.GetIsLeftStance();
                doesVictimHaveMountAgent = victimAgent.HasMount;
                flag5 = (victimAgent.RiderAgent != null);
                isVictimRiderAgentSameAsAttackerAgent = (!flag5 && victimAgent.RiderAgent == attackerAgent);
                victimAgentAbsorbedDamageRatio = victimAgent.Monster.AbsorbedDamageRatio;
                num = victimAgent.GetDamageMultiplierForBone(attackCollisionData.CollisionBoneIndex, (DamageTypes)attackCollisionData.DamageType);
                if (!attackCollisionData.IsMissile && (sbyte)attackCollisionData.DamageType == 1)
                {
                    num = (1f + num) * 0.5f;
                }
                victimMovementDirectionAsAngle = victimAgent.MovementDirectionAsAngle;
                victimAgentScale = victimAgent.AgentScale;
                victimAgentWeight = (float)victimAgent.Monster.Weight;
                victimAgentTotalEncumbrance = victimAgent.GetTotalEncumbrance();
                combatDifficultyMultiplier = __instance.GetDamageMultiplierOfCombatDifficulty(victimAgent);
                string name = victimAgent.Name;
                victimAgentName = (((name != null) ? name.ToString() : null) ?? string.Empty);
                victimAgentMovementVelocity = victimAgent.MovementVelocity;
                victimAgentVelocity = victimAgent.Velocity;
                victimAgentPosition = victimAgent.Position;
                victimAgentFlag = victimAgent.GetAgentFlags();
                victimAgentCharacter = victimAgent.Character;
                isVictimAgentRiderAgentIsMine = (flag5 && victimAgent.RiderAgent.IsMine);
                if (victimAgent.MountAgent != null)
                {
                    Vec2 movementVelocity = victimAgent.MountAgent.MovementVelocity;
                }
            }
            float attackerMovementDirectionAsAngle = 0f;
            float attackerAgentMountChargeDamageProperty = 0f;
            bool doesAttackerHaveMountAgent = false;
            bool isAttackerAgentMine = false;
            bool flag6 = false;
            bool isAttackerAgentRiderAgentIsMine = false;
            bool isAttackerAgentMount = false;
            bool isAttackerAgentHuman = false;
            bool flag7 = false;
            bool isAttackerAgentCharging = false;
            Vec2 attackerAgentMovementVelocity = default(Vec2);
            BasicCharacterObject attackerAgentCharacter = new BasicCharacterObject();
            Vec3 attackerAgentMovementDirection = default(Vec3);
            Vec3 attackerAgentVelocity = default(Vec3);
            Vec3 attackerAgentCurrentWeaponOffset = default(Vec3);
            bool isAttackerAIControlled = false;
            if (!flag)
            {
                doesAttackerHaveMountAgent = attackerAgent.HasMount;
                bool isMainAgent2 = attackerAgent.IsMainAgent;
                isAttackerAgentMine = attackerAgent.IsMine;
                isAttackerAgentMount = attackerAgent.IsMount;
                isAttackerAgentHuman = attackerAgent.IsHuman;
                flag7 = attackerAgent.IsActive();
                isAttackerAgentCharging = attackerAgent.IsCharging;
                flag6 = (attackerAgent.RiderAgent != null);
                isAttackerAIControlled = attackerAgent.IsAIControlled;
                attackerMovementDirectionAsAngle = attackerAgent.MovementDirectionAsAngle;
                attackerAgentMountChargeDamageProperty = attackerAgent.GetAgentDrivenPropertyValue(DrivenProperty.MountChargeDamage);
                attackerAgentMovementVelocity = attackerAgent.MovementVelocity;
                attackerAgentMovementDirection = attackerAgent.GetMovementDirection();
                attackerAgentVelocity = attackerAgent.Velocity;
                if (flag7)
                {
                    attackerAgentCurrentWeaponOffset = attackerAgent.GetCurWeaponOffset();
                }
                attackerAgentCharacter = attackerAgent.Character;
                if (flag6)
                {
                    isAttackerAgentRiderAgentIsMine = attackerAgent.RiderAgent.IsMine;
                }
                if (attackerAgent.MountAgent != null)
                {
                    Vec2 movementVelocity2 = attackerAgent.MountAgent.MovementVelocity;
                }
            }
            bool canGiveDamageToAgentShield = true;
            if (!flag3)
            {
                canGiveDamageToAgentShield = Extensions.CanGiveDamageToAgentShield(attackerAgent, victimAgent);
            }
            CombatLogData combatLog;


            GetAttackCollisionResults(missile, isHorseArcher, armorAmountFloat, shieldOnBack, victimAgentFlag, victimAgentAbsorbedDamageRatio, num, combatDifficultyMultiplier, victimShield, canGiveDamageToAgentShield, isVictimAgentLeftStance, isFriendlyFire, doesAttackerHaveMountAgent, doesVictimHaveMountAgent, attackerAgentMovementVelocity, attackerAgentMountMovementDirection, attackerMovementDirectionAsAngle, victimAgentMovementVelocity, victimAgentMountMovementDirection, victimMovementDirectionAsAngle, flag3, isAttackerAgentMine, flag6, isAttackerAgentRiderAgentIsMine, isAttackerAgentMount, isVictimAgentMine, flag5, isVictimAgentRiderAgentIsMine, isVictimAgentMount, flag, isAttackerAIControlled, attackerAgentCharacter, victimAgentCharacter, attackerAgentMovementDirection, attackerAgentVelocity, attackerAgentMountChargeDamageProperty, attackerAgentCurrentWeaponOffset, isAttackerAgentHuman, flag7, isAttackerAgentCharging, flag2, victimAgentScale, victimAgentWeight, victimAgentTotalEncumbrance, isVictimAgentHuman, victimAgentVelocity, victimAgentPosition, weaponAttachBoneIndex, offHandItem, isHeadShot, crushedThrough, isVictimRiderAgentSameAsAttackerAgent, momentumRemaining, ref attackCollisionData, cancelDamage, victimAgentName, out combatLog);
            combatLog.BodyPartHit = attackCollisionData.VictimHitBodyPart;
            combatLog.IsFatalDamage = (victimAgent != null && victimAgent.Health - (float)attackCollisionData.InflictedDamage <= 0f);
            combatLog.IsVictimEntity = (hitObject != null);

            Extensions.PrintAttackCollisionResults(attackerAgent, victimAgent, hitObject, attackCollisionData, combatLog, cancelDamage);

            return false;
        }


        public static void GetAttackCollisionResults(Mission.Missile missile, bool isHorseArcher, float armorAmountFloat, WeaponComponentData shieldOnBack, AgentFlag victimAgentFlag, float victimAgentAbsorbedDamageRatio, float damageMultiplierOfBone, float combatDifficultyMultiplier, MissionWeapon victimShield, bool canGiveDamageToAgentShield, bool isVictimAgentLeftStance, bool isFriendlyFire, bool doesAttackerHaveMountAgent, bool doesVictimHaveMountAgent, Vec2 attackerAgentMovementVelocity, Vec3 attackerAgentMountMovementDirection, float attackerMovementDirectionAsAngle, Vec2 victimAgentMovementVelocity, Vec3 victimAgentMountMovementDirection, float victimMovementDirectionAsAngle, bool isVictimAgentSameWithAttackerAgent, bool isAttackerAgentMine, bool isAttackerAgentHasRiderAgent, bool isAttackerAgentRiderAgentIsMine, bool isAttackerAgentMount, bool isVictimAgentMine, bool isVictimAgentHasRiderAgent, bool isVictimAgentRiderAgentIsMine, bool isVictimAgentMount, bool isAttackerAgentNull, bool isAttackerAIControlled, BasicCharacterObject attackerAgentCharacter, BasicCharacterObject victimAgentCharacter, Vec3 attackerAgentMovementDirection, Vec3 attackerAgentVelocity, float attackerAgentMountChargeDamageProperty, Vec3 attackerAgentCurrentWeaponOffset, bool isAttackerAgentHuman, bool isAttackerAgentActive, bool isAttackerAgentCharging, bool isVictimAgentNull, float victimAgentScale, float victimAgentWeight, float victimAgentTotalEncumbrance, bool isVictimAgentHuman, Vec3 victimAgentVelocity, Vec3 victimAgentPosition, int weaponAttachBoneIndex, MissionWeapon offHandItem, bool isHeadShot, bool crushedThrough, bool IsVictimRiderAgentSameAsAttackerAgent, float momentumRemaining, ref AttackCollisionData attackCollisionData, bool cancelDamage, string victimAgentName, out CombatLogData combatLog)
        {
            float distance = 0f;
            if (attackCollisionData.IsMissile)
            {
                distance = (attackCollisionData.MissileStartingPosition - attackCollisionData.CollisionGlobalPosition).Length;
            }
            combatLog = new CombatLogData(isVictimAgentSameWithAttackerAgent, isAttackerAgentHuman, isAttackerAgentMine, isAttackerAgentHasRiderAgent, isAttackerAgentRiderAgentIsMine, isAttackerAgentMount, isVictimAgentHuman, isVictimAgentMine, false, isVictimAgentHasRiderAgent, isVictimAgentRiderAgentIsMine, isVictimAgentMount, false, IsVictimRiderAgentSameAsAttackerAgent, false, false, distance);
            ItemObject itemFromWeaponKind = ItemObject.GetItemFromWeaponKind(attackCollisionData.AffectorWeaponKind);
            WeaponComponentData weaponComponentData = (itemFromWeaponKind != null) ? itemFromWeaponKind.GetWeaponWithUsageIndex(attackCollisionData.CurrentUsageIndex) : null;
            bool flag = Extensions.HitWithAnotherBone(ref attackCollisionData, weaponAttachBoneIndex);
            Vec3 agentVelocityContribution = Extensions.GetAgentVelocityContribution(doesAttackerHaveMountAgent, attackerAgentMovementVelocity, attackerAgentMountMovementDirection, attackerMovementDirectionAsAngle);
            Vec3 agentVelocityContribution2 = Extensions.GetAgentVelocityContribution(doesVictimHaveMountAgent, victimAgentMovementVelocity, victimAgentMountMovementDirection, victimMovementDirectionAsAngle);
            if (attackCollisionData.IsColliderAgent)
            {
                combatLog.IsRangedAttack = attackCollisionData.IsMissile;
                combatLog.HitSpeed = (attackCollisionData.IsMissile ? (agentVelocityContribution2 - attackCollisionData.MissileVelocity).Length : (agentVelocityContribution - agentVelocityContribution2).Length);
            }
            float baseMagnitude;
            int speedBonus;
            Mission.ComputeBlowMagnitude(ref attackCollisionData, doesVictimHaveMountAgent, weaponComponentData, isAttackerAgentNull, attackerAgentCharacter, attackerAgentMovementDirection, attackerAgentVelocity, attackerAgentMountChargeDamageProperty, attackerAgentCurrentWeaponOffset, isAttackerAgentHuman, isAttackerAgentActive, isAttackerAgentCharging, isVictimAgentNull, victimAgentScale, victimAgentWeight, victimAgentTotalEncumbrance, isVictimAgentHuman, victimAgentVelocity, victimAgentPosition, momentumRemaining, cancelDamage, flag, attackCollisionData.MissileTotalDamage, agentVelocityContribution, agentVelocityContribution2, out attackCollisionData.BaseMagnitude, out baseMagnitude, out attackCollisionData.MovementSpeedDamageModifier, out speedBonus);
            DamageTypes damageType = (itemFromWeaponKind == null || flag || attackCollisionData.IsAlternativeAttack || attackCollisionData.IsFallDamage || attackCollisionData.IsHorseCharge) ? DamageTypes.Blunt : ((DamageTypes)attackCollisionData.DamageType);
            combatLog.DamageType = damageType;
            if (!attackCollisionData.IsColliderAgent && attackCollisionData.EntityExists)
            {
                string name = PhysicsMaterial.GetFromIndex(attackCollisionData.PhysicsMaterialIndex).Name;
                bool isWoodenBody = name == "wood" || name == "wood_weapon" || name == "wood_shield";
                attackCollisionData.BaseMagnitude *= Extensions.GetEntityDamageMultiplier(isAttackerAgentCharging, weaponComponentData, damageType, isWoodenBody);
                attackCollisionData.InflictedDamage = MBMath.ClampInt((int)attackCollisionData.BaseMagnitude, 0, 2000);
                combatLog.InflictedDamage = attackCollisionData.InflictedDamage;
            }
            int num = 0;
            if (attackCollisionData.IsColliderAgent && !isVictimAgentNull)
            {
                if (attackCollisionData.IsAlternativeAttack)
                {
                    baseMagnitude = attackCollisionData.BaseMagnitude;
                }
                if (attackCollisionData.AttackBlockedWithShield)
                {
                    Mission.ComputeBlowDamageOnShield(isAttackerAgentNull, isAttackerAgentActive, isAttackerAgentCharging, canGiveDamageToAgentShield, isVictimAgentLeftStance, victimShield, ref attackCollisionData, weaponComponentData, attackCollisionData.BaseMagnitude);
                    attackCollisionData.AbsorbedByArmor = attackCollisionData.InflictedDamage;
                }
                else
                {
                    Mission.ComputeBlowDamage(armorAmountFloat, shieldOnBack, victimAgentFlag, victimAgentAbsorbedDamageRatio, damageMultiplierOfBone, combatDifficultyMultiplier, damageType, baseMagnitude, attackCollisionData.CollisionGlobalPosition, itemFromWeaponKind, attackCollisionData.AttackBlockedWithShield, attackCollisionData.CollidedWithShieldOnBack, speedBonus, cancelDamage, attackCollisionData.IsFallDamage, out attackCollisionData.InflictedDamage, out attackCollisionData.AbsorbedByArmor, out num);
                }



                GCOToolbox.GCOToolbox.ProjectileBalance.ApplyProjectileArmorResistance(armorAmountFloat, ref attackCollisionData, missile, isHorseArcher);


                combatLog.InflictedDamage = attackCollisionData.InflictedDamage;
                combatLog.AbsorbedDamage = attackCollisionData.AbsorbedByArmor;

                //combatLog.AttackProgress = attackCollisionData.AttackProgress;
                //issionGameModels.Current.AgentApplyDamageModel.CalculateDamage(attackerAgentCharacter, victimAgentCharacter, offHandItem, isHeadShot, isVictimAgentMount, isVictimAgentHuman, doesAttackerHaveMountAgent, isVictimAgentNull, isAttackerAgentHuman, attackCollisionData, weaponComponentData);
                //combatLog.ExtraDamage = attackCollisionData.InflictedDamage - combatLog.InflictedDamage;
            }
            if (!attackCollisionData.IsFallDamage && isFriendlyFire && !isAttackerAIControlled && GameNetwork.IsSessionActive)
            {
                int num2 = attackCollisionData.IsMissile ? MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) : MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
                attackCollisionData.SelfInflictedDamage = MBMath.Round((float)attackCollisionData.InflictedDamage * ((float)num2 * 0.01f));
                int num3 = attackCollisionData.IsMissile ? MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) : MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
                attackCollisionData.InflictedDamage = MBMath.Round((float)attackCollisionData.InflictedDamage * ((float)num3 * 0.01f));
                if (isVictimAgentMount && !doesAttackerHaveMountAgent)
                {
                    attackCollisionData.InflictedDamage = MBMath.Round((float)attackCollisionData.InflictedDamage * 0.1f);
                }
                combatLog.InflictedDamage = attackCollisionData.InflictedDamage;
                combatLog.IsFriendlyFire = true;
            }
            if (attackCollisionData.AttackBlockedWithShield && attackCollisionData.InflictedDamage > 0 && (int)victimShield.HitPoints - attackCollisionData.InflictedDamage <= 0)
            {
                attackCollisionData.IsShieldBroken = true;
            }
        }
    }

     static class Extensions
    {
        internal static float GetEntityDamageMultiplier(bool isAttackerAgentCharging, WeaponComponentData weapon, DamageTypes damageType, bool isWoodenBody)
        {
            float num = 1f;
            if (isAttackerAgentCharging)
            {
                num *= 0.2f;
            }
            if (weapon != null)
            {
                if (weapon.WeaponFlags.HasAnyFlag(WeaponFlags.BonusAgainstShield))
                {
                    num *= 1.2f;
                }
                switch (damageType)
                {
                    case DamageTypes.Cut:
                        num *= 0.8f;
                        break;
                    case DamageTypes.Pierce:
                        num *= 0.1f;
                        break;
                }
                if (isWoodenBody && weapon.WeaponFlags.HasAnyFlag(WeaponFlags.Burning))
                {
                    num *= 1.5f;
                }
            }
            return num;
        }
        internal static bool HitWithAnotherBone(ref AttackCollisionData collisionData, int weaponAttachBoneIndex)
        {
            return collisionData.AttackBoneIndex != -1 && weaponAttachBoneIndex != -1 && weaponAttachBoneIndex != (int)collisionData.AttackBoneIndex;
        }
        internal static Vec3 GetAgentVelocityContribution(bool hasAgentMountAgent, Vec2 agentMovementVelocity, Vec3 agentMountMovementDirection, float agentMovementDirectionAsAngle)
        {
            Vec3 result = Vec3.Zero;
            if (hasAgentMountAgent)
            {
                result = agentMovementVelocity.y * agentMountMovementDirection;
            }
            else
            {
                result.AsVec2 = agentMovementVelocity;
                result.RotateAboutZ(agentMovementDirectionAsAngle);
            }
            return result;
        }

        internal static bool CanGiveDamageToAgentShield(Agent attacker, Agent defender)
        {
            if (GameNetwork.IsSessionActive && (MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) > 0 || MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) > 0) && Mission.Current.Mode != MissionMode.Duel)
            {
                if (attacker != null)
                {
                    bool flag = attacker.Controller == Agent.ControllerType.AI;
                }
            }
            return !CancelsDamageAndBlocksAttackBecauseOfNonEnemyCase(attacker, defender);
        }

        private static bool CancelsDamageAndBlocksAttackBecauseOfNonEnemyCase(Agent attacker, Agent victim)
        {
            if (victim == null || attacker == null)
            {
                return false;
            }
            bool flag = !GameNetwork.IsSessionActive || (MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) <= 0 && MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) <= 0) || Mission.Current.Mode == MissionMode.Duel || attacker.Controller == Agent.ControllerType.AI;
            bool flag2 = attacker.IsFriendOf(victim);
            return (flag && flag2) || (victim.IsHuman && !flag2 && !attacker.IsEnemyOf(victim));
        }

        public static void PrintAttackCollisionResults(Agent attackerAgent, Agent victimAgent, GameEntity hitEntity, AttackCollisionData attackCollisionData, CombatLogData combatLog, bool cancelDamage)
        {
            bool flag = attackCollisionData.CollisionBoneIndex == -1;
            if (!cancelDamage && attackCollisionData.IsColliderAgent && attackCollisionData.InflictedDamage > 0 && !flag && !attackCollisionData.AttackBlockedWithShield && victimAgent.State == AgentState.Active)
            {
                MissionReversePatches.AddCombatLogSafe(Mission.Current, attackerAgent, victimAgent, hitEntity, combatLog);
            }
        }
    }
}

