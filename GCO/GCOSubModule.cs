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

namespace GCO
{
    public class GCOSubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            Config.initConfig();

            Harmony harmony = new Harmony("GIRUCombatOverhaul");
            harmony.PatchAll(typeof(GCOSubModule).Assembly);

        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            InformationManager.DisplayMessage(new InformationMessage("Loaded GCO", Color.White));

            if (!Config.ConfigLoadedSuccessfully)
            {
                InformationManager.DisplayMessage(new InformationMessage("Unable to read GCO Config - defaulting settings", Color.White));
            }
        }


        private void AddBehaviors(CampaignGameStarter gameInitializer)
        {
            if (Config.ConfigSettings.SimplifiedBanditLogicEnabled)
            {
                gameInitializer.AddBehavior(new CampaignLogic());
            }
        }

        private bool xorberaxCompatibilityCheck()
        {
            bool xorberaxcutthrougheveryoneExists = false;
            var methodBases = Harmony.GetAllPatchedMethods().ToList<MethodBase>();

            foreach (var method in methodBases)
            {
                if (Harmony.GetPatchInfo(method).Owners.Contains("xorberax.cutthrougheveryone"))
                {
                    xorberaxcutthrougheveryoneExists = true;
                }
            }

            return xorberaxcutthrougheveryoneExists;
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            Campaign campaign = game.GameType as Campaign;

            bool xorbarexExists = xorberaxCompatibilityCheck();

            if (xorbarexExists)
            {
                Config.ConfigSettings.CleaveEnabled = false;
                Config.xorbarexCleaveExists = true;
                InformationManager.DisplayMessage(new InformationMessage("Xorbarex Cut Through Everyone installation detected", Color.White));
            }
            else
            {
                Config.xorbarexCleaveExists = false;
            }
        }



        #region essential placeholder code to prevent crashing



        public override void OnCampaignStart(Game game, object starterObject)
        {
            if (game.GameType is Campaign)
            {
                CampaignGameStarter gameInitializer = (CampaignGameStarter)starterObject;
                this.AddBehaviors(gameInitializer);
            }
        }

        public override void OnGameInitializationFinished(Game game)
        {
            Campaign campaign = game.GameType as Campaign;
        }

        public override void OnGameLoaded(Game game, object initializerObject)
        {
            if (game.GameType is Campaign)
            {
                CampaignGameStarter gameInitializer = (CampaignGameStarter)initializerObject;
                this.AddBehaviors(gameInitializer);
            }
        }

        public override void OnNewGameCreated(Game game, object initializerObject)
        {
        }

        #endregion essential placeholder code to prevent crashing
    }
}


