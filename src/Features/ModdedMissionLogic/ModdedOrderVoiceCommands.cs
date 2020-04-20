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
            switch (formation.InitialClass)
            {
                case FormationClass.Infantry:
                case FormationClass.HeavyInfantry:
                    QueueClass.QueueItem("Infantry");
                    return false;
                case FormationClass.Ranged:
                case FormationClass.NumberOfDefaultFormations:
                    QueueClass.QueueItem("Archers");
                    return false;
                case FormationClass.Cavalry:
                case FormationClass.LightCavalry:
                case FormationClass.HeavyCavalry:
                    QueueClass.QueueItem("Cavalry");
                    return false;
                case FormationClass.HorseArcher:
                    QueueClass.QueueItem("HorseArchers");
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

    public static class QueueClass
    {
        private static readonly Queue<string> queue = new Queue<string>();

        private static MissionTime VoiceCommandTimer = MissionTime.Now;

        private static void ResetVoiceCommandTimer()
        {
            VoiceCommandTimer = MissionTime.SecondsFromNow(2f);
        }

        public static MissionTime GetVoiceCommandTimer()
        {
            return VoiceCommandTimer;
        }
        public static void QueueItem(string voiceTypeString)
        {
            queue.Enqueue(voiceTypeString);
        }

        public static Queue<string> GetVoiceCommandQueue()
        {
            return queue;
        }

        public static string GetNextQueueItem()
        {
            ResetVoiceCommandTimer();
            return queue.Dequeue();
        }
    }

    public static class ModdedOrderVoiceCaller
    {
        public static bool AfterSetOrderMakeVoice(OrderType orderType)
        {
       
            ////testing
            //Agent.Main.MakeVoice("Move, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
            //return false;

            if (!Mission.Current.IsOrderShoutingAllowed())
            {
                return false;
            }
            switch (orderType)
            {
                case OrderType.Move:
                case OrderType.MoveToLineSegment:
                case OrderType.MoveToLineSegmentWithHorizontalLayout:
                    QueueClass.QueueItem("Move");
                    return false;
                case OrderType.Charge:
                case OrderType.ChargeWithTarget:
                    QueueClass.QueueItem("Charge");
                    return false;
                case OrderType.StandYourGround:
                    QueueClass.QueueItem("Stop");
                    return false;
                case OrderType.FollowMe:
                    QueueClass.QueueItem("Follow");
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
                    QueueClass.QueueItem("Retreat");
                    return false;
                case OrderType.AdvanceTenPaces:
                case OrderType.Advance:
                    QueueClass.QueueItem("Advance");
                    return false;
                case OrderType.FallBackTenPaces:
                case OrderType.FallBack:
                    QueueClass.QueueItem("FallBack");
                    return false;
                case OrderType.LookAtEnemy:
                    QueueClass.QueueItem("FaceEnemy");
                    return false;
                case OrderType.LookAtDirection:
                    QueueClass.QueueItem("FaceDirection");
                    break;
                case OrderType.ArrangementLine:
                    QueueClass.QueueItem("FormLine");
                    return false;
                case OrderType.ArrangementCloseOrder:
                    QueueClass.QueueItem("FormShieldWall");
                    return false;
                case OrderType.ArrangementLoose:
                    QueueClass.QueueItem("FormLoose");
                    return false;
                case OrderType.ArrangementCircular:
                    QueueClass.QueueItem("FormCircle");
                    return false;
                case OrderType.ArrangementSchiltron:
                    QueueClass.QueueItem("FormSquare");
                    return false;
                case OrderType.ArrangementVee:
                    QueueClass.QueueItem("FormSkein");
                    return false;
                case OrderType.ArrangementColumn:
                    QueueClass.QueueItem("FormColumn");
                    return false;
                case OrderType.ArrangementScatter:
                    QueueClass.QueueItem("FormScatter");
                    return false;
                case OrderType.HoldFire:
                    QueueClass.QueueItem("HoldFire");
                    return false;
                case OrderType.FireAtWill:
                    QueueClass.QueueItem("FireAtWill");
                    return false;
                case OrderType.Mount:
                    QueueClass.QueueItem("Mount");
                    return false;
                case OrderType.Dismount:
                    QueueClass.QueueItem("Dismount");
                    return false;
                case OrderType.AIControlOn:
                    QueueClass.QueueItem("CommandDelegate");
                    return false;
                case OrderType.AIControlOff:
                    QueueClass.QueueItem("CommandUndelegate");
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

