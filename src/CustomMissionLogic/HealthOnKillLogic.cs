using GCO.ModOptions;
using System;
using System.Diagnostics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace GCO.Features.CustomMissionLogic
{
    public class HealthOnKillLogic : MissionLogic
    {
        public HealthOnKillLogic() : base()
        {

        }

        public bool CheckPlayerAlive()
        {
            return Mission.MainAgent != null && Mission.MainAgent.IsActive();
        }

        private float GetMedicineSkillCalculation()
        {
            float medicineSkill = 0f;
            try
            {
                medicineSkill = (int)Math.Floor((double)Hero.MainHero.GetSkillValue(DefaultSkills.Medicine));
                medicineSkill = (float)medicineSkill * Config.ConfigSettings.HPOnKillMedicineLevelScalePercentage * 0.01f;
            }
            catch { }

            return medicineSkill;
        }

        public override void OnAgentHit(Agent affAgent1, Agent affAgent2, int damage, int weaponKind, int currentWeaponUsageIndex)
        {
            if (!affAgent1.IsHuman) return;

            float HPOnKillAmount = Config.ConfigSettings.HPOnKillAmount;
            float healAmount = HPOnKillAmount + GetMedicineSkillCalculation();

            bool valid = CheckPlayerAlive() && affAgent1.IsHuman && affAgent2 == Mission.MainAgent && damage > 0 && affAgent2 != affAgent1;

            if (valid && affAgent1.Health <= 0f)
            {
                Mission.MainAgent.Health = Math.Min(Mission.MainAgent.Health + HPOnKillAmount, Mission.MainAgent.HealthLimit);

                InformationManager.DisplayMessage(new InformationMessage($"Healed {(int)healAmount} health!", Colors.White));
            }
        }
    }
}
