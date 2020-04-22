using HarmonyLib;
using NetworkMessages.FromClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using static TaleWorlds.MountAndBlade.SkinVoiceManager;

namespace GCO.Features.ModdedMissionLogic
{

    class ModdedOrderVoiceCommands
    {
        private static bool SelectFormationMakeVoicePrefix(Formation formation, Agent agent)
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
                    QueueClass.QueueItem("Infantry", delay);
                    return false;
                case FormationClass.Ranged:
                case FormationClass.NumberOfDefaultFormations:
                    QueueClass.QueueItem("Archers", delay);
                    return false;
                case FormationClass.Cavalry:
                case FormationClass.LightCavalry:
                case FormationClass.HeavyCavalry:
                    QueueClass.QueueItem("Cavalry", delay);
                    return false;
                case FormationClass.HorseArcher:
                    QueueClass.QueueItem("HorseArchers", delay + 400f);
                    return false;
                default:
                    return false;
            }
        }

        #region SelectAllFormations and Victory bugfix
        private static bool SelectAllFormationsPrefix(ref OrderController __instance, Agent selectorAgent, bool uiFeedback)
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
                (f => ModdedOrderVoiceExtensions.IsFormationSelectable(f, selectorAgent))))
            {
                thisFormations.Add(formation);
            };

            return false;
        }

        private static bool ChooseWeaponToCheerWithCheerAndUpdateTimerPrefix(KeyValuePair<Agent, RandomTimer> kvp)
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
                    key.SetActionChannel(1, ModdedOrderVoiceExtensions._cheerActions[MBRandom.RandomInt(ModdedOrderVoiceExtensions._cheerActions.Length)], false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
                    key.MakeVoice(voiceType, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
                    kvp.Value.Reset(Mission.Current.Time);
                    kvp.Value.ChangeDuration(6f, 12f);
                }
            }
            return false;
        }
        #endregion SelectAllFormations and Victory bugfix

        private static bool AfterSetOrderMakeVoicePrefix(OrderType orderType, Agent agent)
        {
            ModdedOrderVoiceCaller.AfterSetOrderMakeVoice(orderType);

            return false;

        }
    }

    internal static class ModdedOrderVoiceExtensions
    {
        internal static readonly ActionIndexCache[] _cheerActions = new ActionIndexCache[]
        {
            ActionIndexCache.Create("act_cheer_1"),
            ActionIndexCache.Create("act_cheer_2"),
            ActionIndexCache.Create("act_cheer_3"),
            ActionIndexCache.Create("act_cheer_4")
        };

        internal static ObservableCollection<Formation> GetSelectedFormations(this OrderController __instance)
        {
            return Traverse.Create(__instance).Field<ObservableCollection<Formation>>("selectedFormations").Value;

        }

        internal static bool IsFormationSelectable(Formation formation, Agent selectorAgent)
        {
            return (selectorAgent == null || formation.PlayerOwner == selectorAgent) && !formation.Units.IsEmpty<Agent>();
        }
        internal static Team GetTeam(this OrderController __instance)
        {
            return Traverse.Create(__instance).Field<Team>("team").Value;
        }
    }

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
    internal class QueueItem
    {
        public string VoiceTypeString { get; }

        public float DelayAfter { get; }

        public QueueItem(string voiceTypeString, float delayAfter)
        {
            VoiceTypeString = voiceTypeString;
            DelayAfter = delayAfter;
        }

        public static bool operator ==(QueueItem left, QueueItem right)
        {
            return left.VoiceTypeString == right.VoiceTypeString && left.DelayAfter == right.DelayAfter;
        }

        public static bool operator !=(QueueItem left, QueueItem right)
        {
            return left.VoiceTypeString != right.VoiceTypeString || left.DelayAfter != right.DelayAfter;
        }
    }

    internal static class QueueClass
    {
        private static readonly Queue<QueueItem> queue = new Queue<QueueItem>();
        private static MissionTime VoiceCommandTimer = MissionTime.Now;

        internal static void ResetVoiceCommandTimer(float delay = 2000f)
        {
            VoiceCommandTimer = MissionTime.MillisecondsFromNow(delay);
        }

        internal static MissionTime GetVoiceCommandTimer()
        {
            return VoiceCommandTimer;
        }
        internal static void QueueItem(string voiceTypeString, float delayAfter = 2000f)
        {

            if (queue.Count > 7)
            {
                _ = queue.Dequeue();
            }

            queue.Enqueue(new QueueItem(voiceTypeString, delayAfter));
        }

        internal static Queue<QueueItem> GetVoiceCommandQueue()
        {
            return queue;
        }

        internal static QueueItem GetNextQueueItem()
        {
            var item = queue.Dequeue();
            while (queue.Count > 0 && queue.Peek() == item)
                return GetNextQueueItem();
            return item;
        }
    }


    internal static class ModdedOrderVoiceCaller
    {
        internal static bool AfterSetOrderMakeVoice(OrderType orderType)
        {
            if (!Mission.Current.IsOrderShoutingAllowed())
            {
                return false;
            }


            // var test = Mission.Current.MainAgent.GetAgentVoiceDefinition();
            switch (orderType)
            {
                case OrderType.Move:
                case OrderType.MoveToLineSegment:
                case OrderType.MoveToLineSegmentWithHorizontalLayout:
                    QueueClass.QueueItem("Move", 800f);
                    return false;
                case OrderType.Charge:
                case OrderType.ChargeWithTarget:
                    QueueClass.QueueItem("Charge", 800f);
                    return false;
                case OrderType.StandYourGround:
                    QueueClass.QueueItem("Stop", 800f);
                    return false;
                case OrderType.FollowMe:
                    QueueClass.QueueItem("Follow", 800f);
                    return false;
                case OrderType.FollowEntity:
                case OrderType.GuardMe:
                case OrderType.Attach:
                case OrderType.FormCustom:
                case OrderType.FormDeep:
                case OrderType.FormWide:
                case OrderType.FormWider:
                case OrderType.CohesionHigh:
                case OrderType.CohesionMedium:
                case OrderType.CohesionLow:
                case OrderType.RideFree:
                case OrderType.UseAnyWeapon:
                case OrderType.UseBluntWeaponsOnly:
                    break;
                case OrderType.Retreat:
                    QueueClass.QueueItem("Retreat", 1200f);
                    return false;
                case OrderType.AdvanceTenPaces:
                case OrderType.Advance:
                    QueueClass.QueueItem("Advance", 1200f);
                    return false;
                case OrderType.FallBackTenPaces:
                case OrderType.FallBack:
                    QueueClass.QueueItem("FallBack", 1200f);
                    return false;
                case OrderType.LookAtEnemy:
                    QueueClass.QueueItem("FaceEnemy", 1200f);
                    return false;
                case OrderType.LookAtDirection:
                    QueueClass.QueueItem("FaceDirection", 1200f);
                    break;
                case OrderType.ArrangementLine:
                    QueueClass.QueueItem("FormLine", 1200f);
                    return false;
                case OrderType.ArrangementCloseOrder:
                    QueueClass.QueueItem("FormShieldWall", 1200f);
                    return false;
                case OrderType.ArrangementLoose:
                    QueueClass.QueueItem("FormLoose", 1200f);
                    return false;
                case OrderType.ArrangementCircular:
                    QueueClass.QueueItem("FormCircle", 1200f);
                    return false;
                case OrderType.ArrangementSchiltron:
                    QueueClass.QueueItem("FormSquare", 1200f);
                    return false;
                case OrderType.ArrangementVee:
                    QueueClass.QueueItem("FormSkein", 1200f);
                    return false;
                case OrderType.ArrangementColumn:
                    QueueClass.QueueItem("FormColumn", 1200f);
                    return false;
                case OrderType.ArrangementScatter:
                    QueueClass.QueueItem("FormScatter", 1200f);
                    return false;
                case OrderType.HoldFire:
                    QueueClass.QueueItem("HoldFire", 1200f);
                    return false;
                case OrderType.FireAtWill:
                    QueueClass.QueueItem("FireAtWill", 1200f);
                    return false;
                case OrderType.Mount:
                    QueueClass.QueueItem("Mount", 1200f);
                    return false;
                case OrderType.Dismount:
                    QueueClass.QueueItem("Dismount", 1200f);
                    return false;
                case OrderType.AIControlOn:
                    QueueClass.QueueItem("CommandDelegate", 1200f);
                    return false;
                case OrderType.AIControlOff:
                    QueueClass.QueueItem("CommandUndelegate", 1200f);
                    return false;
                default:
                    return false;
            }

            return false;
        }
    }
}

