using HarmonyLib;
using System;
using System.Collections.Generic;
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
    [HarmonyPatch(typeof(OrderController))]
    class ModdedOrderVoiceCommands
    {
        [HarmonyPrefix]
        [HarmonyPatch("SelectFormationMakeVoice")]
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

        [HarmonyPrefix]
        [HarmonyPatch("AfterSetOrderMakeVoice")]
        private static bool AfterSetOrderMakeVoicePrefix(OrderType orderType, Agent agent)
        {
            ModdedOrderVoiceCaller.AfterSetOrderMakeVoice(orderType);

            return false;

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

          // var test = Mission.Current.MainAgent.GetAgentVoiceDefinition();
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
            switch (orderType)
            {
                case OrderType.Move:
                case OrderType.MoveToLineSegment:
                case OrderType.MoveToLineSegmentWithHorizontalLayout:
                    QueueClass.QueueItem("Move", 650f);
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
                    QueueClass.QueueItem("Retreat", 800f);
                    return false;
                case OrderType.AdvanceTenPaces:
                case OrderType.Advance:
                    QueueClass.QueueItem("Advance", 800f);
                    return false;
                case OrderType.FallBackTenPaces:
                case OrderType.FallBack:
                    QueueClass.QueueItem("FallBack", 800f);
                    return false;
                case OrderType.LookAtEnemy:
                    QueueClass.QueueItem("FaceEnemy", 800f);
                    return false;
                case OrderType.LookAtDirection:
                    QueueClass.QueueItem("FaceDirection", 800f);
                    break;
                case OrderType.ArrangementLine:
                    QueueClass.QueueItem("FormLine", 800f);
                    return false;
                case OrderType.ArrangementCloseOrder:
                    QueueClass.QueueItem("FormShieldWall", 800f);
                    return false;
                case OrderType.ArrangementLoose:
                    QueueClass.QueueItem("FormLoose", 800f);
                    return false;
                case OrderType.ArrangementCircular:
                    QueueClass.QueueItem("FormCircle", 800f);
                    return false;
                case OrderType.ArrangementSchiltron:
                    QueueClass.QueueItem("FormSquare", 800f);
                    return false;
                case OrderType.ArrangementVee:
                    QueueClass.QueueItem("FormSkein", 800f);
                    return false;
                case OrderType.ArrangementColumn:
                    QueueClass.QueueItem("FormColumn", 800f);
                    return false;
                case OrderType.ArrangementScatter:
                    QueueClass.QueueItem("FormScatter", 800f);
                    return false;
                case OrderType.HoldFire:
                    QueueClass.QueueItem("HoldFire", 800f);
                    return false;
                case OrderType.FireAtWill:
                    QueueClass.QueueItem("FireAtWill", 800f);
                    return false;
                case OrderType.Mount:
                    QueueClass.QueueItem("Mount", 800f);
                    return false;
                case OrderType.Dismount:
                    QueueClass.QueueItem("Dismount", 800f);
                    return false;
                case OrderType.AIControlOn:
                    QueueClass.QueueItem("CommandDelegate", 800f);
                    return false;
                case OrderType.AIControlOff:
                    QueueClass.QueueItem("CommandUndelegate", 800f);
                    return false;
                default:
                    return false;
            }

            return false;
        }
    }
}

