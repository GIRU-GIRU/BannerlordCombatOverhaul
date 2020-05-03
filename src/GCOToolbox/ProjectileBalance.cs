using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCO.GCOMissionLogic;
using GCO.ModOptions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace GCO.GCOToolbox
{
    partial class GCOToolbox
    {
        public partial class ProjectileBalance
        {
            internal static void ApplyProjectileArmorResistance(float finputArmor, ref AttackCollisionData collisionData, Mission.Missile missile, bool isHorseArcher)
            {
                int inputArmor = (int)finputArmor;
                bool multiplierHeadOrNeckShot = collisionData.VictimHitBodyPart == BoneBodyPartType.Head || collisionData.VictimHitBodyPart == BoneBodyPartType.Neck;

                var type = missile.Weapon.CurrentUsageItem.WeaponClass;
                switch (type)
                {
                    case WeaponClass.Arrow:
                        ApplyArrowArmorPen(ref collisionData, inputArmor, multiplierHeadOrNeckShot);
                        break;
                    case WeaponClass.Bolt:
                        ApplyBoltArmorPen(ref collisionData, inputArmor, multiplierHeadOrNeckShot);
                        break;

                    case WeaponClass.ThrowingAxe:
                        ApplyThrowableArmorPen(ref collisionData, inputArmor, multiplierHeadOrNeckShot);
                        break;
                    case WeaponClass.ThrowingKnife:
                        ApplyThrowableArmorPen(ref collisionData, inputArmor, multiplierHeadOrNeckShot);
                        break;
                    case WeaponClass.Javelin:
                        ApplyThrowableArmorPen(ref collisionData, inputArmor, multiplierHeadOrNeckShot);
                        break;
                    default:
                        break;
                }

                if (isHorseArcher)
                {
                    collisionData.InflictedDamage = 80;
                }
            }

            internal static bool CheckForHorseArcher(Agent victim)
            {
                bool isHorseArcher = false;

                if (victim != null && !victim.IsMount && !victim.WieldedWeapon.IsEmpty && victim.WieldedWeapon.Weapons != null)
                {
                    if (!victim.IsMainAgent)
                    {
                        bool hasBowAndArrows = victim.WieldedWeapon.Weapons.Any(x =>
                            x.AmmoClass == WeaponClass.Bow || x.AmmoClass == WeaponClass.Arrow);

                        isHorseArcher = victim.HasMount && hasBowAndArrows;
                    }
                }

                return isHorseArcher;
            }

            internal static bool ApplyHorseCrippleLogic(Agent victim, BoneBodyPartType victimHitBodyPart)
            {
                bool makesRear = false;

                if (Config.ConfigSettings.HorseProjectileCrippleEnabled)
                {
                    if (victim != null && victim.IsMount)
                    {
                        if (victim.RiderAgent != null && !victim.RiderAgent.IsMainAgent)
                        {
                            if (victimHitBodyPart == BoneBodyPartType.Head || victimHitBodyPart == BoneBodyPartType.Neck)
                            {
                                makesRear = true;
                            }
                            else
                            {
                                try
                                {
                                    if (victim.IsActive())
                                    {

                                        victim.RiderAgent.AgentDrivenProperties.MountSpeed = 0f;                                    
                                        HorseCrippleLogic.CrippleHorseNew(victim, MissionTime.SecondsFromNow((float)Config.ConfigSettings.HorseProjectileCrippleDuration));
                                        victim.UpdateAgentStats();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    InformationManager.DisplayMessage(
                                     new InformationMessage("Cripple Horse Error " + ex.Message, Colors.White));
                                }
                            }
                        }
                    }
                }
                return makesRear;
            }

            internal static int IfCrossbowEmpowerStat(double skillAmount, SkillObject skill)
            {

                if (skill == DefaultSkills.Crossbow)
                {
                    skillAmount *= 1.3;
                }

                return (int)Math.Round(skillAmount);
            }

            private static void ApplyThrowableArmorPen(ref AttackCollisionData collisionData, int inputArmor, bool multiplierHeadOrNeckShot)
            {
                var inflictedDamage = collisionData.InflictedDamage;

                inflictedDamage -= inputArmor;
                inflictedDamage = Math.Max(inflictedDamage, 25);

                if (multiplierHeadOrNeckShot)
                {
                    if (inputArmor >= 20)
                    {
                        inflictedDamage = Math.Min(inflictedDamage, 60);
                    }
                }
                else
                {
                    if (inputArmor >= 20) inflictedDamage = Math.Min(inflictedDamage, 40);
                    if (inputArmor >= 30) inflictedDamage = Math.Min(inflictedDamage, 35);
                    if (inputArmor >= 40) inflictedDamage = Math.Min(inflictedDamage, 30);
                }


                collisionData.InflictedDamage = inflictedDamage;
            }

            private static void ApplyBoltArmorPen(ref AttackCollisionData collisionData, int inputArmor, bool multiplierHeadOrNeckShot)
            {
                var inflictedDamage = collisionData.InflictedDamage;
                var inputArmorReduced = (int)Math.Round(inputArmor * 0.7);

                inflictedDamage -= inputArmorReduced;
                inflictedDamage = Math.Max(inflictedDamage, 35);

                if (multiplierHeadOrNeckShot)
                {
                    if (inputArmor >= 20)
                    {
                        inflictedDamage = Math.Min(inflictedDamage, 65);
                    }
                }
                else
                {
                    if (inputArmor >= 20) inflictedDamage = Math.Min(inflictedDamage, 45);
                    if (inputArmor >= 30) inflictedDamage = Math.Min(inflictedDamage, 40);
                    if (inputArmor >= 40) inflictedDamage = Math.Min(inflictedDamage, 35);
                }


                collisionData.InflictedDamage = inflictedDamage;
            }

            private static void ApplyArrowArmorPen(ref AttackCollisionData collisionData, int inputArmor, bool multiplierHeadOrNeckShot)
            {
                var inflictedDamage = collisionData.InflictedDamage;

                inflictedDamage -= inputArmor;
                inflictedDamage = Math.Max(inflictedDamage, 25);

                if (multiplierHeadOrNeckShot)
                {
                    if (inputArmor >= 15)
                    {
                        inflictedDamage = Math.Min(inflictedDamage, 55);
                    }
                }
                else
                {
                    if (inputArmor >= 20) inflictedDamage = Math.Min(inflictedDamage, 35);
                    if (inputArmor >= 30) inflictedDamage = Math.Min(inflictedDamage, 30);
                    if (inputArmor >= 40) inflictedDamage = Math.Min(inflictedDamage, 25);
                }


                collisionData.InflictedDamage = inflictedDamage;
            }

            internal static void CheckForProjectileFlinch(ref Blow b, ref Blow blow, AttackCollisionData collisionData, Agent victim)
            {
                if (Config.ConfigSettings.ProjectileBalancingEnabled && GCOToolbox.CheckIfAliveHero(victim))
                {
                    if (victim != null && b.IsMissile())
                    {
                        if (collisionData.VictimHitBodyPart != BoneBodyPartType.Head && collisionData.VictimHitBodyPart != BoneBodyPartType.Neck)
                        {
                            var projStunThresholdMultiplier = Config.ConfigSettings.ProjectileStunPercentageThreshold / 100;

                            if (b.InflictedDamage < (victim.HealthLimit * projStunThresholdMultiplier))
                            {
                                b.BlowFlag |= BlowFlags.ShrugOff;
                                blow.BlowFlag |= BlowFlags.ShrugOff;
                            }
                        }
                    }
                }
            }
        }
    }
}
