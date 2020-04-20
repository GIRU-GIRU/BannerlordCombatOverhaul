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
            float delay = 0.2f;
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
                    QueueClass.QueueItem("HorseArchers", delay);
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

            return true;

        }
    }

    internal class QueueItem
    {
        public string VoiceTypeString { get; }

        public float DelayAfter { get; }

        public QueueItem(string voiceTypeString, float delayAfter)
        {
            VoiceTypeString = voiceTypeString;
            DelayAfter = delayAfter;
        }
    }

    internal static class QueueClass
    {
        private static readonly Queue<QueueItem> queue = new Queue<QueueItem>();
        static Random rand = new Random();
        private static MissionTime VoiceCommandTimer = MissionTime.Now;

        internal static void ResetVoiceCommandTimer(float delay = 2000f)
        {
            VoiceCommandTimer = MissionTime.MillisecondsFromNow(delay);
            EasterEgg.DONT_SHOW_THIS_ON_STREAM();
        }

        internal static MissionTime GetVoiceCommandTimer()
        {
            return VoiceCommandTimer;
        }
        internal static void QueueItem(string voiceTypeString, float delayAfter = 2000f)
        {
            queue.Enqueue(new QueueItem(voiceTypeString, delayAfter));
        }

        internal static Queue<QueueItem> GetVoiceCommandQueue()
        {
            return queue;
        }

        internal static QueueItem GetNextQueueItem()
        {
            return queue.Dequeue();
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
                    QueueClass.QueueItem("Move", 600f);
                    return false;
                case OrderType.Charge:
                case OrderType.ChargeWithTarget:
                    QueueClass.QueueItem("Charge", 0.3f);
                    return false;
                case OrderType.StandYourGround:
                    QueueClass.QueueItem("Stop", 0.2f);
                    return false;
                case OrderType.FollowMe:
                    QueueClass.QueueItem("Follow", 0.2f);
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
                    QueueClass.QueueItem("Retreat", 0.2f);
                    return false;
                case OrderType.AdvanceTenPaces:
                case OrderType.Advance:
                    QueueClass.QueueItem("Advance", 0.2f);
                    return false;
                case OrderType.FallBackTenPaces:
                case OrderType.FallBack:
                    QueueClass.QueueItem("FallBack", 0.2f);
                    return false;
                case OrderType.LookAtEnemy:
                    QueueClass.QueueItem("FaceEnemy", 0.2f);
                    return false;
                case OrderType.LookAtDirection:
                    QueueClass.QueueItem("FaceDirection", 0.2f);
                    break;
                case OrderType.ArrangementLine:
                    QueueClass.QueueItem("FormLine", 0.3f);
                    return false;
                case OrderType.ArrangementCloseOrder:
                    QueueClass.QueueItem("FormShieldWall", 0.3f);
                    return false;
                case OrderType.ArrangementLoose:
                    QueueClass.QueueItem("FormLoose", 0.3f);
                    return false;
                case OrderType.ArrangementCircular:
                    QueueClass.QueueItem("FormCircle", 0.3f);
                    return false;
                case OrderType.ArrangementSchiltron:
                    QueueClass.QueueItem("FormSquare", 0.3f);
                    return false;
                case OrderType.ArrangementVee:
                    QueueClass.QueueItem("FormSkein", 0.3f);
                    return false;
                case OrderType.ArrangementColumn:
                    QueueClass.QueueItem("FormColumn", 0.3f);
                    return false;
                case OrderType.ArrangementScatter:
                    QueueClass.QueueItem("FormScatter", 0.3f);
                    return false;
                case OrderType.HoldFire:
                    QueueClass.QueueItem("HoldFire", 0.3f);
                    return false;
                case OrderType.FireAtWill:
                    QueueClass.QueueItem("FireAtWill", 0.3f);
                    return false;
                case OrderType.Mount:
                    QueueClass.QueueItem("Mount", 0.3f);
                    return false;
                case OrderType.Dismount:
                    QueueClass.QueueItem("Dismount", 0.3f);
                    return false;
                case OrderType.AIControlOn:
                    QueueClass.QueueItem("CommandDelegate", 0.3f);
                    return false;
                case OrderType.AIControlOff:
                    QueueClass.QueueItem("CommandUndelegate", 0.3f);
                    return false;
                default:
                    return false;
            }


            return false;
        }
    }




















    internal class EasterEgg
    {
        static readonly Random rand = new Random();
        internal static void DONT_SHOW_THIS_ON_STREAM()
        {
            if (Agent.Main.Name.IndexOf("grichie", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var val = rand.Next(0, 100);
                if (val >= 90)
                {
                    InformationManager.DisplayMessage(new InformationMessage("STFU grichie_", new Color(1, 0, 0)));
                }
            }
        }
    }
}

