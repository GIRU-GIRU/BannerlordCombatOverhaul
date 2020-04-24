using GCO.ModOptions;
using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace GCO.Features
{
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

        public class ProjectileBalance
        {

            internal static void ApplyProjectileArmorResistance(float finputArmor, ref AttackCollisionData collisionData, Mission.Missile missile)
            {
                var inflictedDamage = collisionData.InflictedDamage;
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

            private static void ApplyArrowArmorPen(ref AttackCollisionData collisionData, int inputArmor, bool multiplierHeadOrNeckShot)
            {
                var inflictedDamage = collisionData.InflictedDamage;

                inflictedDamage -= inputArmor;
                inflictedDamage = Math.Max(inflictedDamage, 15);

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

        public class MeleeBalance
        {

            internal static float GCOGetStaticFlinchPeriod(Agent attackerAgent, float defenderStunPeriod)
            {
                if (attackerAgent == Mission.Current.MainAgent)
                {
                    return 0.6f;
                };

                return defenderStunPeriod;
            }

            

            internal static void CheckToAddHyperarmor(ref Blow b, ref Blow blow)
            {
                if (IsHyperArmorActive())
                {
                    b.BlowFlag |= BlowFlags.ShrugOff;
                    blow.BlowFlag |= BlowFlags.ShrugOff;
                    InformationManager.DisplayMessage(
                         new InformationMessage("Player hyperarmor prevented flinch!", Colors.White));
                }
            }

            internal static void CreateHyperArmorBuff(Agent defenderAgent)
            {
                if (defenderAgent.IsPlayerControlled)
                {
                    ApplyHyperArmor();
                }
            }

          

            private static MissionTime _playerAgentHyperarmorActiveTime;

            private static void ApplyHyperArmor()
            {
                _playerAgentHyperarmorActiveTime = MissionTime.SecondsFromNow(Config.ConfigSettings.HyperArmorDuration);
            }

            private static bool IsHyperArmorActive()
            {
                bool hyperArmorActive = !_playerAgentHyperarmorActiveTime.IsPast;

                return hyperArmorActive;
            }
        }
    }
}