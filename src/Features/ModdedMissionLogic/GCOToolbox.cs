using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace GCO.Features.ModdedMissionLogic
{
    public static class GCOToolbox
    {

        internal static float GCOGetStaticFlinchPeriod(Agent attackerAgent, float defenderStunPeriod)
        {
            if (attackerAgent == Mission.Current.MainAgent)
            {
                return 0.6f;
            };

            return defenderStunPeriod;
        }

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
                InformationManager.DisplayMessage(
                     new InformationMessage("Player hyperarmor prevented flinch!", Colors.White));
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
