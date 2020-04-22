using System;
using System.Collections.Generic;
using TaleWorlds.MountAndBlade;
using System.Linq;
using System.Text;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem;
using HarmonyLib;
using GCO.Features.ModdedMissionLogic;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using TaleWorlds.Library;
using Helpers;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using GCO.Features.ModdedWorldMapLogic;

namespace GCO
{
    public class GCOSubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {           
            Harmony harmony = new Harmony("GIRUCombatOverhaul");

            Config.initConfig();
            Config.ConfigureHarmonyPatches(ref harmony);

            harmony.PatchAll(typeof(GCOSubModule).Assembly);
        }
        public override void OnMissionBehaviourInitialize(Mission mission)
        {
            if (mission.IsOrderShoutingAllowed())
            {
                if (Config.ConfigSettings.OrderVoiceCommandQueuing) mission.AddMissionBehaviour(new QueuedVoiceLogic());
            }
            base.OnMissionBehaviourInitialize(mission);
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            InformationManager.DisplayMessage(new InformationMessage("Loaded GCO", Color.White));

            if (!Config.ConfigLoadedSuccessfully)
            {
                InformationManager.DisplayMessage(new InformationMessage("Unable to read GCO Config - defaulting settings", Color.White));
            }
        }

        public override void OnCampaignStart(Game game, object starterObject)
        {
            if (game.GameType is Campaign)
            {
                CampaignGameStarter gameInitializer = (CampaignGameStarter)starterObject;
                this.AddBehaviors(gameInitializer);
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            bool flag = game.GameType is Campaign;
            if (flag)
            {
                CampaignGameStarter gameStarterObject2 = (CampaignGameStarter)gameStarterObject;
                this.AddBehaviors(gameStarterObject2);
            }
        }

        private void AddBehaviors(CampaignGameStarter gameInitializer)
        {
            gameInitializer.AddBehavior(new CampaignLogic());
        } 
    }
}


