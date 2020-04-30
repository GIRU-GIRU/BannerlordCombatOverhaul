using GCO.CopiedLogic;
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

        #region SelectAllFormations and Victory bugfix
        internal static bool SelectAllFormationsPrefix(ref OrderController __instance, Agent selectorAgent, bool uiFeedback)
        {
            var voiceType = new SkinVoiceType("Everyone");

            if (GameNetwork.IsClient)
            {
                GameNetwork.BeginModuleEventAsClient();
                GameNetwork.WriteMessage(new SelectAllFormations());
                GameNetwork.EndModuleEventAsClient();
            }
            if (uiFeedback && !GameNetwork.IsClientOrReplay && selectorAgent != null && Mission.Current.IsOrderShoutingAllowed())
            {
                selectorAgent.MakeVoice(voiceType, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
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

        internal static bool ChooseWeaponToCheerWithCheerAndUpdateTimerPrefix(ref OrderController __instance, Agent cheerAgent, out bool resetTimer)
        {
            resetTimer = false;
            if (cheerAgent.GetCurrentActionType(1) != Agent.ActionCodeType.EquipUnequip)
            {
                EquipmentIndex wieldedItemIndex = cheerAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
                bool flag = wieldedItemIndex != EquipmentIndex.None && !cheerAgent.Equipment[wieldedItemIndex].CurrentUsageItem.Item.ItemFlags.HasAnyFlag(ItemFlags.DropOnAnyAction);
                if (!flag)
                {
                    EquipmentIndex equipmentIndex = EquipmentIndex.None;
                    for (EquipmentIndex equipmentIndex2 = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex2 < EquipmentIndex.Weapon4; equipmentIndex2++)
                    {
                        if (!cheerAgent.Equipment[equipmentIndex2].IsEmpty && !cheerAgent.Equipment[equipmentIndex2].CurrentUsageItem.Item.ItemFlags.HasAnyFlag(ItemFlags.DropOnAnyAction))
                        {
                            equipmentIndex = equipmentIndex2;
                            break;
                        }
                    }
                    if (equipmentIndex == EquipmentIndex.None)
                    {
                        if (wieldedItemIndex != EquipmentIndex.None)
                        {
                            cheerAgent.TryToSheathWeaponInHand(Agent.HandIndex.MainHand, Agent.WeaponWieldActionType.WithAnimation);
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                    else
                    {
                        cheerAgent.TryToWieldWeaponInSlot(equipmentIndex, Agent.WeaponWieldActionType.WithAnimation, false);
                    }
                }
                if (flag)
                {
                    var voiceType = new SkinVoiceType("Victory");
                    cheerAgent.SetActionChannel(1, OrderControllerExtensions.CheerActions[MBRandom.RandomInt(OrderControllerExtensions.CheerActions.Length)], false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
                    cheerAgent.MakeVoice(voiceType, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
                    resetTimer = true;
                }              
            }

            return false;
        }
        #endregion SelectAllFormations and Victory bugfix

        internal static bool AfterSetOrderMakeVoicePrefix(OrderType orderType, Agent agent)
        {
            OrderControllerExtensions.AfterSetOrderMakeVoice(orderType, agent);

            return false;
        }
    }
}
