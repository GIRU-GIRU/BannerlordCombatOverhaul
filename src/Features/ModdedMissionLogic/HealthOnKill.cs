using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace GCO.Features.ModdedMissionLogic
{
    public class HealthOnKill : TaleWorlds.MountAndBlade.MissionLogic
    {
        public bool CheckPlayerAlive()
        {
            return Mission.MainAgent != null && Mission.MainAgent.IsActive();
        }


        private float GetMedicineSkillCalculation()
        {
            int medicineSkill = (int)Math.Floor((double)Hero.MainHero.GetSkillValue(DefaultSkills.Medicine));

            return (float)medicineSkill * 0.1f;
        }

        public override void OnAgentHit(Agent affAgent1, Agent affAgent2, int damage, int weaponKind, int currentWeaponUsageIndex)
        {
            if (!affAgent1.IsHuman) return;


            float HPOnKillAmount = Config.ConfigSettings.HPOnKillAmount;
            float healAmount = HPOnKillAmount + GetMedicineSkillCalculation();


            bool valid = this.CheckPlayerAlive() && affAgent1.IsHuman && affAgent2 == Mission.MainAgent && damage > 0;

            if (valid && affAgent1.Health <= 0f)
            {
                Mission.MainAgent.Health = Math.Min(Mission.MainAgent.Health + HPOnKillAmount, Mission.MainAgent.HealthLimit);


                InformationManager.DisplayMessage(new InformationMessage($"Healed {(int)healAmount} health!", Colors.White));
            }
        }
    }
}
