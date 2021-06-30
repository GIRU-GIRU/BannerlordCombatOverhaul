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
                    collisionData.InflictedDamage *= 2;
                }
            }

            internal static bool CheckForHorseArcher(Agent victim)
            {
                bool isHorseArcher = false;


                if (victim != null && !victim.IsMount)
                {
                    if (victim.HasWeapon() && !victim.WieldedWeapon.IsEmpty && victim.WieldedWeapon.Weapons != null)
                    {
                        if (!victim.IsMainAgent && victim.WieldedWeapon)
                        {
                            bool hasBowAndArrows = victim.WieldedWeapon.Weapons.Any(x =>
                                x.AmmoClass == WeaponClass.Bow || x.AmmoClass == WeaponClass.Arrow);

                            isHorseArcher = victim.HasMount && hasBowAndArrows;
                        }
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
                                makesRear = Config.ConfigSettings.HorseHeadshotRearingEnabled;
                            }
                            else
                            {
                                if (victim.IsActive() && victim.RiderAgent)
                                {
                                    victim.RiderAgent.AgentDrivenProperties.MountSpeed = 0f;
                                    HorseCrippleLogic.CrippleHorseNew(victim, MissionTime.SecondsFromNow((float)Config.ConfigSettings.HorseProjectileCrippleDuration));
                                    victim.UpdateAgentStats();
                                }
                            }
                        }
                    }
                }

                return makesRear;
            }


            private static double CrosspowerStatEmpowermentMultiplier = 1.3;
            internal static int IfCrossbowEmpowerStat(double skillAmount, SkillObject skill)
            {

                if (skill == DefaultSkills.Crossbow)
                {
                    skillAmount *= CrosspowerStatEmpowermentMultiplier;
                }

                return (int)Math.Round(skillAmount);
            }


          
            //TODO move to config

            private static int ThrowableHeadshotArmorThreshold = 15;
            private static int ThrowableDefaultDamageMin = 25;
            private static int ThrowableDefaultDamageMax = 55;
                               
            private static int ThrowableArmorThresholdLight = 20;
            private static int ThrowableArmorThresholdLightDamage = 45;
                               
            private static int ThrowableArmorThresholdMed = 30;
            private static int ThrowableArmorThresholdMedDamage = 40;
                               
            private static int ThrowableArmorThresholdHeavy = 40;
            private static int ThrowableArmorThresholdHeavyDamage = 35;
            private static void ApplyThrowableArmorPen(ref AttackCollisionData collisionData, int inputArmor, bool multiplierHeadOrNeckShot)
            {
                var inflictedDamage = collisionData.InflictedDamage;

                inflictedDamage -= inputArmor;
                inflictedDamage = Math.Max(inflictedDamage, ThrowableDefaultDamageMin);

                if (multiplierHeadOrNeckShot)
                {
                    if (inputArmor >= ThrowableHeadshotArmorThreshold)
                    {
                        inflictedDamage = Math.Min(inflictedDamage, ThrowableDefaultDamageMax);
                    }
                }
                else
                {
                    if (inputArmor >= ThrowableArmorThresholdLight) inflictedDamage = Math.Min(inflictedDamage, ThrowableArmorThresholdLightDamage);
                    if (inputArmor >= ThrowableArmorThresholdMed) inflictedDamage = Math.Min(inflictedDamage, ThrowableArmorThresholdMedDamage);
                    if (inputArmor >= ThrowableArmorThresholdHeavy) inflictedDamage = Math.Min(inflictedDamage, ThrowableArmorThresholdHeavyDamage);
                }


                collisionData.InflictedDamage = inflictedDamage;
            }

            //TODO move to config
            private static double ArmorReductionMultiplier = 0.7;

            private static int CrossbowHeadshotArmorThreshold = 15;
            private static int CrossbowDefaultDamageMin = 25;
            private static int CrossbowDefaultDamageMax = 55;

            private static int CrossbowArmorThresholdLight = 20;
            private static int CrossbowArmorThresholdLightDamage = 45;

            private static int CrossbowArmorThresholdMed = 30;
            private static int CrossbowArmorThresholdMedDamage = 40;
                         
            private static int CrossbowArmorThresholdHeavy = 40;
            private static int CrossbowArmorThresholdHeavyDamage = 35;

            private static void ApplyBoltArmorPen(ref AttackCollisionData collisionData, int inputArmor, bool multiplierHeadOrNeckShot)
            {
                var inflictedDamage = collisionData.InflictedDamage;
                var inputArmorReduced = (int)Math.Round(inputArmor * ArmorReductionMultiplier);

                inflictedDamage -= inputArmorReduced;
                inflictedDamage = Math.Max(inflictedDamage, CrossbowDefaultDamageMin);

                if (multiplierHeadOrNeckShot)
                {
                    if (inputArmor >= CrossbowHeadshotArmorThreshold)
                    {
                        inflictedDamage = Math.Min(inflictedDamage, CrossbowDefaultDamageMax);
                    }
                }
                else
                {
                    if (inputArmor >= CrossbowArmorThresholdLight) inflictedDamage = Math.Min(inflictedDamage, CrossbowArmorThresholdLightDamage);
                    if (inputArmor >= CrossbowArmorThresholdMed) inflictedDamage = Math.Min(inflictedDamage, CrossbowArmorThresholdMedDamage);
                    if (inputArmor >= CrossbowArmorThresholdHeavy) inflictedDamage = Math.Min(inflictedDamage, CrossbowArmorThresholdHeavyDamage);
                }


                collisionData.InflictedDamage = inflictedDamage;
            }


            //TODO move to config
            private static int BowHeadshotArmorThreshold = 15;
            private static int BowDefaultDamageMin = 25;
            private static int BowDefaultDamageMax = 55;

            private static int BowArmorThresholdLight = 20;
            private static int BowArmorThresholdLightDamage = 35;
                         
            private static int BowArmorThresholdMed = 30;
            private static int BowArmorThresholdMedDamage = 30;
                         
            private static int BowArmorThresholdHeavy = 40;
            private static int BowArmorThresholdHeavyDamage = 25;

            private static void ApplyArrowArmorPen(ref AttackCollisionData collisionData, int inputArmor, bool multiplierHeadOrNeckShot)
            {
                var inflictedDamage = collisionData.InflictedDamage;

                inflictedDamage -= inputArmor;
                inflictedDamage = Math.Max(inflictedDamage, BowDefaultDamageMin);

                if (multiplierHeadOrNeckShot)
                {
                    if (inputArmor >= BowHeadshotArmorThreshold)
                    {
                        inflictedDamage = Math.Min(inflictedDamage, BowDefaultDamageMax);
                    }
                }
                else
                {
                    if (inputArmor >= BowArmorThresholdLight) inflictedDamage = Math.Min(inflictedDamage, BowArmorThresholdLightDamage);
                    if (inputArmor >= BowArmorThresholdMed) inflictedDamage = Math.Min(inflictedDamage, BowArmorThresholdMedDamage);
                    if (inputArmor >= BowArmorThresholdHeavy) inflictedDamage = Math.Min(inflictedDamage, BowArmorThresholdHeavyDamage);
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
