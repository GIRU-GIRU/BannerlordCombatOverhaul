using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;
using HarmonyLib;
using TaleWorlds.Library;
using GCO.ModOptions;
using GCO.Features.CustomMissionLogic;
using GCO.Utility;
using System.Linq;
using GCO.CustomMissionLogic;

namespace GCO
{
    public class GCOSubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            Harmony harmony = new Harmony("GIRUCombatOverhaul");

            CompatibilityCheck.CheckAndApply();

            Config.InitConfig();

            harmony.PatchAll(typeof(GCOSubModule).Assembly);
            var methodss = harmony.GetPatchedMethods().ToList();
            Config.ConfigureHarmonyPatches(ref harmony);

        }

        public override void OnMissionBehaviourInitialize(Mission mission)
        {
            ConfigureHealthOnkillLogic(mission);
            ConfigureQueuedVoiceLogic(mission);
            ConfigureHorseCrippleLogic(mission);


            base.OnMissionBehaviourInitialize(mission);
        }

        private void ConfigureQueuedVoiceLogic(Mission mission)
        {
            if (Config.ConfigSettings.OrderVoiceCommandQueuing && mission.IsOrderShoutingAllowed())
            {
                mission.AddMissionBehaviour(new QueuedVoiceLogic());
            }
        }

        private void ConfigureHorseCrippleLogic(Mission mission)
        {
            if(Config.ConfigSettings.ProjectileBalancingEnabled)
            {
                mission.AddMissionBehaviour(new HorseCrippleLogic());
            }

        }

        private void ConfigureHealthOnkillLogic(Mission mission)
        {
            if (Config.ConfigSettings.HPOnKillEnabled)
            {
                if (mission.Scene != null) // why is this needed?
                {
                    bool isCombat = mission.CombatType == Mission.MissionCombatType.Combat;
                    bool isArenaCombat = mission.CombatType == Mission.MissionCombatType.ArenaCombat;
                    if (mission.IsFieldBattle || isCombat || isArenaCombat)
                    {
                        mission.AddMissionBehaviour(new HealthOnKillLogic());
                    }
                }
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            InformationManager.DisplayMessage(new InformationMessage("Loaded GCO 2.0.0", Color.White));

            if (!Config.ConfigLoadedSuccessfully)
            {
                InformationManager.DisplayMessage(new InformationMessage("Unable to read GCO Config - defaulting settings", Color.White));
            }
        }
    }
}


