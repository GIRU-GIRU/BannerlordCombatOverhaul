using GCO.ModOptions;
using System;
using System.Diagnostics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace GCO.GCOMissionLogic
{
    public class HealthOnKillLogic : MissionLogic
    {
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
                if (Config.ConfigSettings.HPOnKillMedicineLevelScalePercentage > 0)
                {
                    medicineSkill = (float)medicineSkill * Config.ConfigSettings.HPOnKillMedicineLevelScalePercentage * 0.01f;
                }
            }
            catch { }

            return medicineSkill;
        }

        public override void OnAgentHit(Agent affAgent1, Agent affAgent2, int damage, int weaponKind, int currentWeaponUsageIndex)
        {
            if (affAgent1.IsHuman && damage > 0 && affAgent2 != affAgent1)
            {
                if (affAgent1.Health <= 0f)
                {
                    bool isMainAgent = CheckPlayerAlive() && affAgent2 == Mission.MainAgent;

                    if (isMainAgent)
                    {
                        float healAmount = Config.ConfigSettings.HPOnKillAmount + GetMedicineSkillCalculation();
                        Mission.MainAgent.Health = Math.Min(Mission.MainAgent.Health + Config.ConfigSettings.HPOnKillAmount, Mission.MainAgent.HealthLimit);

                        InformationManager.DisplayMessage(new InformationMessage($"Healed {(int)healAmount} health!", Colors.White));
                    }
                    else if(Config.ConfigSettings.HPOnKillForAI && affAgent2.IsActive())
                    {
                       affAgent2.Health = Math.Min(affAgent2.Health + Config.ConfigSettings.HPOnKillAmount, affAgent2.HealthLimit);
                    }
                }              
            }           
        }
    }
}
