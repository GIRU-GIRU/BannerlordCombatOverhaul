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
                    VoiceCommandQueue.QueueItem("HorseArchers", delay + 400f);
                    return false;
                default:
                    return false;
            }
        }

        #region SelectAllFormations and Victory bugfix
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
                var voiceType = new SkinVoiceType("Everyone");
                selectorAgent.MakeVoice(voiceType, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
            }
            __instance.GetSelectedFormations().Clear();

            IEnumerable<Formation> formations = __instance.GetTeam().Formations;

            var thisFormations = __instance.GetSelectedFormations();
            foreach (var formation in formations.Where((Func<Formation, bool>)
                (f => OrderControllerExtensions.IsFormationSelectable(f, selectorAgent))))
            {
                thisFormations.Add(formation);
            };

            return false;
        }

        internal static bool ChooseWeaponToCheerWithCheerAndUpdateTimerPrefix(KeyValuePair<Agent, RandomTimer> kvp)
        {
            Agent key = kvp.Key;
            if (key.GetCurrentActionType(1) != Agent.ActionCodeType.EquipUnequip)
            {
                EquipmentIndex wieldedItemIndex = key.GetWieldedItemIndex(Agent.HandIndex.MainHand);
                bool flag = wieldedItemIndex != EquipmentIndex.None && !key.Equipment[wieldedItemIndex].CurrentUsageItem.Item.ItemFlags.HasAnyFlag(ItemFlags.DropOnAnyAction);
                if (!flag)
                {
                    EquipmentIndex equipmentIndex = EquipmentIndex.None;
                    for (EquipmentIndex equipmentIndex2 = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex2 < EquipmentIndex.Weapon4; equipmentIndex2++)
                    {
                        if (!key.Equipment[equipmentIndex2].IsEmpty && !key.Equipment[equipmentIndex2].CurrentUsageItem.Item.ItemFlags.HasAnyFlag(ItemFlags.DropOnAnyAction))
                        {
                            equipmentIndex = equipmentIndex2;
                            break;
                        }
                    }
                    if (equipmentIndex == EquipmentIndex.None)
                    {
                        if (wieldedItemIndex != EquipmentIndex.None)
                        {
                            key.TryToSheathWeaponInHand(Agent.HandIndex.MainHand, Agent.WeaponWieldActionType.WithAnimation);
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                    else
                    {
                        key.TryToWieldWeaponInSlot(equipmentIndex, Agent.WeaponWieldActionType.WithAnimation, false);
                    }
                }
                if (flag)
                {
                    var voiceType = new SkinVoiceType("Victory");
                    key.SetActionChannel(1, OrderControllerExtensions.CheerActions[MBRandom.RandomInt(OrderControllerExtensions.CheerActions.Length)], false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
                    key.MakeVoice(voiceType, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
                    kvp.Value.Reset(Mission.Current.Time);
                    kvp.Value.ChangeDuration(6f, 12f);
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
