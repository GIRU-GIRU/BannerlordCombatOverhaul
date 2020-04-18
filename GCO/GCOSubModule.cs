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

            if (CompatibilityCheck())
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

        private void AddBehaviors(CampaignGameStarter gameInitializer)
        {
            if (Config.ConfigSettings.SimplifiedSurrenderLogic)
            {
                gameInitializer.AddBehavior(new CampaignLogic());
            }
        }

        private bool CompatibilityCheck()
        {
            var methodBases = Harmony.GetAllPatchedMethods().ToList<MethodBase>();

            foreach (var method in methodBases)
            {
                if (Harmony.GetPatchInfo(method).Owners.Contains("xorberax.cutthrougheveryone"))
                {
                    return true;
                }
            }

            return false;
        }

    }
}


