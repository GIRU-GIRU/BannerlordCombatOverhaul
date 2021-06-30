using GCO.GCOMissionLogic;
using GCO.ReversePatches;
using GCO.Utility;
using NetworkMessages.FromClient;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Screen;
using static TaleWorlds.MountAndBlade.SkinVoiceManager;

namespace GCO.Patches
{
    internal static class OrderControllerPatches
    {
        internal static bool SelectFormationMakeVoicePrefix(Formation formation, Agent agent)
        {
            if (!Mission.Current.IsOrderShoutingAllowed())
            {
                return false;
            }

            float delay = 800f;
            switch (formation.InitialClass)
            {
                case FormationClass.Infantry:
                case FormationClass.HeavyInfantry:
                    VoiceCommandQueue.QueueItem("Infantry", delay);
                    return false;
                case FormationClass.Ranged:
                case FormationClass.NumberOfDefaultFormations:
                    VoiceCommandQueue.QueueItem("Archers", delay);
                    return false;
                case FormationClass.Cavalry:
                case FormationClass.LightCavalry:
                case FormationClass.HeavyCavalry:
                    VoiceCommandQueue.QueueItem("Cavalry", delay);
                    return false;
                case FormationClass.HorseArcher:
                    VoiceCommandQueue.QueueItem("HorseArchers", delay + 800f);
                    return false;
                default:
                    return false;
            }
        }


        //transpiler method investigate
        internal static bool SelectAllFormationsPrefix(ref OrderController __instance, Agent selectorAgent, bool uiFeedback)
        {

            if (GameNetwork.IsClient)
            {
                GameNetwork.BeginModuleEventAsClient();
                GameNetwork.WriteMessage(new SelectAllFormations());
                GameNetwork.EndModuleEventAsClient();
            }
            if (uiFeedback && !GameNetwork.IsClientOrReplay && selectorAgent != null && Mission.Current.IsOrderShoutingAllowed())
            {
                VoiceCommandQueue.QueueItem("Everyone", 800f);
            }

            var _selectedFormations = MissionAccessTools.Get_selectedFormations(ref __instance);
            _selectedFormations.Clear();

            var _team = MissionAccessTools.Get_team(ref __instance);
            IEnumerable<Formation> formations = _team.Formations;
            foreach (Formation formation in _team.Formations.Where<Formation>((Func<Formation, bool>)(f => IsFormationSelectable(f, selectorAgent))))
                _selectedFormations.Add(formation);

            OrderControllerReversePatches.OnSelectedFormationsCollectionChanged(__instance);

            return false;
        }

        private static bool IsFormationSelectable(Formation formation, Agent selectorAgent)
        {
            return (selectorAgent == null || formation.PlayerOwner == selectorAgent) && formation.CountOfUnits > 0;
        }

       
        internal static bool AfterSetOrderMakeVoicePrefix(OrderType orderType, Agent agent)
        {
            OrderControllerLogic.AfterSetOrderMakeVoice(orderType, agent);

            return false;
        }
    }
}
