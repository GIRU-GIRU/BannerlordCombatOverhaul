using GCO.GCOMissionLogic;
using GCO.ReversePatches;
using HarmonyLib;
using Helpers;
using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace GCO.Patches
{

    [HarmonyPatch]
    public static class SandboxAgentStatCalculateModelPatches
    {
        [HarmonyPatch(typeof(SandboxAgentStatCalculateModel), "UpdateHorseStats")]
        [HarmonyPrefix]
        private static bool UpdateHorseStatsPrefix(ref SandboxAgentStatCalculateModel __instance, Agent agent, AgentDrivenProperties agentDrivenProperties)
        {
            Equipment spawnEquipment = agent.SpawnEquipment;
            agentDrivenProperties.AiSpeciesIndex = (int)spawnEquipment[EquipmentIndex.ArmorItemEndSlot].Item.Id.InternalValue;
            agentDrivenProperties.AttributeRiding = 0.8f + ((spawnEquipment[EquipmentIndex.HorseHarness].Item != null) ? 0.2f : 0f);
            float num = 0f;
            for (int i = 1; i < 12; i++)
            {
                if (spawnEquipment[i].Item != null)
                {
                    num += (float)spawnEquipment[i].GetBodyArmorHorse();
                }
            }
            agentDrivenProperties.ArmorTorso = num;
            ItemObject item = spawnEquipment[EquipmentIndex.ArmorItemEndSlot].Item;
            if (item != null)
            {
                float num2 = 1f;
                if (!agent.Mission.Scene.IsAtmosphereIndoor)
                {
                    if (agent.Mission.Scene.GetRainDensity() > 0f)
                    {
                        num2 *= 0.9f;
                    }
                    if (CampaignTime.Now.IsNightTime)
                    {
                        num2 *= 0.9f;
                    }
                }
                HorseComponent horseComponent = item.HorseComponent;
                EquipmentElement equipmentElement = spawnEquipment[EquipmentIndex.ArmorItemEndSlot];
                EquipmentElement harness = spawnEquipment[EquipmentIndex.HorseHarness];
                int baseHorseManeuver = equipmentElement.GetBaseHorseManeuver(harness);
                int num3 = equipmentElement.GetBaseHorseSpeed(harness) + 1;
                agentDrivenProperties.MountChargeDamage = (float)equipmentElement.GetBaseHorseCharge(harness) * 0.004f;
                agentDrivenProperties.MountDifficulty = (float)equipmentElement.Item.Difficulty;
                RidingModel ridingModel = Game.Current.BasicModels.RidingModel;
                ItemObject item2 = equipmentElement.Item;
                Agent riderAgent = agent.RiderAgent;
                agentDrivenProperties.TopSpeedReachDuration = ridingModel.CalculateAcceleration(item2, (riderAgent != null) ? riderAgent.Character : null);
                if (agent.RiderAgent != null)
                {
                    ExplainedNumber explainedNumber = new ExplainedNumber((float)baseHorseManeuver, null);
                    ExplainedNumber explainedNumber2 = new ExplainedNumber((float)num3, null);
                    SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Riding, DefaultSkillEffects.HorseManeuver, agent.RiderAgent.Character as CharacterObject, ref explainedNumber, true);
                    SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Riding, DefaultSkillEffects.HorseSpeed, agent.RiderAgent.Character as CharacterObject, ref explainedNumber2, true);
                    if (harness.Item == null)
                    {
                        explainedNumber.AddFactor(-0.1f, null);
                        explainedNumber2.AddFactor(-0.1f, null);
                    }
                    agentDrivenProperties.MountManeuver = explainedNumber.ResultNumber;
                    if (HorseCrippleLogic.CheckHorseCrippled(agent))
                    {
                        agentDrivenProperties.MountSpeed = 1;
                    }
                    else
                    {
                        agentDrivenProperties.MountSpeed = num2 * 0.22f * (1f + explainedNumber2.ResultNumber);
                    }

                    return false; 
                }
                agentDrivenProperties.MountManeuver = (float)baseHorseManeuver;

                if (HorseCrippleLogic.CheckHorseCrippled(agent))
                {
                    agentDrivenProperties.MountSpeed = 1;
                }
                else
                {
                    agentDrivenProperties.MountSpeed = num2 * 0.22f * (float)(1 + num3);
                }
            }

            return false;
        }


        //[HarmonyPatch(typeof(SandboxAgentStatCalculateModel), "UpdateAgentStats")]
        //[HarmonyPostfix]
        //public static void UpdateAgentStatsPostfix(ref SandboxAgentStatCalculateModel __instance, Agent agent, AgentDrivenProperties agentDrivenProperties)
        //{
        //    if (!agent.IsHuman)
        //    {
        //        if (!HorseCrippleLogic.CheckHorseCrippled(agent))
        //        {
        //            UpdateHorseStatsPrefix(ref __instance, agent, agentDrivenProperties);
        //            return;
        //        }
            
        //    }

        //    SandboxAgentStatCalculateModelReversePatches.UpdateHumanStats(__instance, agent, agentDrivenProperties);

        //}


        //[HarmonyPatch(typeof(AgentDrivenProperties), "UpdateDrivenProperties")]
        //[HarmonyPostfix]
        //private static void UpdateDrivenPropertiesPostfix(Agent agent)
        //{
        //    var test = agent;
        //    //   MBAPI.IMBAgent.UpdateDrivenProperties(this.GetPtr(), values);
        //}
    }

}




    //[HarmonyPatch(typeof(SandboxAgentStatCalculateModel), "UpdateDrivenProperties")]
    //class testclasstwo
    //{
    //    [HarmonyPostfix]
    //    private UIntPtr GetPtr()
    //    {
    //        var test = values;
    //        //   MBAPI.IMBAgent.UpdateDrivenProperties(this.GetPtr(), values);
    //    }
    //}


