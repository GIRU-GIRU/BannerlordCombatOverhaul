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
                    QueueClass.QueueItem(() => agent.MakeVoice(SkinVoiceManager.VoiceType.Infantry, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                case FormationClass.Ranged:
                case FormationClass.NumberOfDefaultFormations:
                    QueueClass.QueueItem(() => agent.MakeVoice(SkinVoiceManager.VoiceType.Archers, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                case FormationClass.Cavalry:
                case FormationClass.LightCavalry:
                case FormationClass.HeavyCavalry:
                    QueueClass.QueueItem(() => agent.MakeVoice(SkinVoiceManager.VoiceType.Cavalry, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                case FormationClass.HorseArcher:
                    QueueClass.QueueItem(() => agent.MakeVoice(SkinVoiceManager.VoiceType.HorseArchers, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
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



    [HarmonyPatch(typeof(Mission), "OnTick")]
    public class VoiceCommandTimer
    {

        [HarmonyPostfix]
        internal static void OnTickPostfix(float dt, float realDt, bool updateCamera)
        {
      
            bool initialized = false;
            MissionTime missionTime;

            if (MissionTime.Now != null)
            {              
                initialized = true;
            }

            if (initialized)
            {
                missionTime = MissionTime.SecondsFromNow(5);
                if (missionTime.IsPast)
                {
                    Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.Move, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
                 
                }
            }




            var queue = QueueClass.GetVoiceCommandQueue();
            if (queue.Count > 0)
            {
                var timer = QueueClass.GetVoiceCommandTimer();

                if (timer.IsPast)
                {
                    QueueClass.ExecuteVoiceCommand();

                }
            }
        }
    }
    public static class QueueClass
    {
        private static Queue<Action> queue = new Queue<Action>();

        private static MissionTime VoiceCommandTimer = MissionTime.Now;

        private static void ResetVoiceCommandTimer()
        {
            VoiceCommandTimer = MissionTime.SecondsFromNow(2f);
        }

        public static MissionTime GetVoiceCommandTimer()
        {
            return VoiceCommandTimer;
        }
        public static void QueueItem(Action makeVoice)
        {
            queue.Enqueue(makeVoice);
        }

        public static Queue<Action> GetVoiceCommandQueue()
        {
            return queue;
        }

        public static void ExecuteVoiceCommand()
        {
            var action = queue.Dequeue();
            // action.Invoke();
            ResetVoiceCommandTimer();
        }
    }

    public static class ModdedOrderVoiceCaller
    {
        public static bool AfterSetOrderMakeVoice(OrderType orderType)
        {
       
            //testing
            Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.Move, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
            return false;

            if (!Mission.Current.IsOrderShoutingAllowed())
            {
                return false;
            }
            switch (orderType)
            {
                case OrderType.Move:
                case OrderType.MoveToLineSegment:
                case OrderType.MoveToLineSegmentWithHorizontalLayout:
                    QueueClass.QueueItem(() => Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.Move, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                case OrderType.Charge:
                case OrderType.ChargeWithTarget:
                    QueueClass.QueueItem(() => Mission.Current.MainAgent.MakeVoice(VoiceType.Charge, CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                case OrderType.StandYourGround:
                    QueueClass.QueueItem(() => Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.Stop, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                case OrderType.FollowMe:
                    QueueClass.QueueItem(() => Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.Follow, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
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
                    QueueClass.QueueItem(() => Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.Retreat, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                case OrderType.AdvanceTenPaces:
                case OrderType.Advance:
                    QueueClass.QueueItem(() => Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.Advance, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                case OrderType.FallBackTenPaces:
                case OrderType.FallBack:
                    QueueClass.QueueItem(() => Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.FallBack, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                case OrderType.LookAtEnemy:
                    QueueClass.QueueItem(() => Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.FaceEnemy, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                case OrderType.LookAtDirection:
                    QueueClass.QueueItem(() => Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.FaceDirection, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    break;
                case OrderType.ArrangementLine:
                    QueueClass.QueueItem(() => Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.FormLine, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                case OrderType.ArrangementCloseOrder:
                    QueueClass.QueueItem(() => Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.FormShieldWall, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                case OrderType.ArrangementLoose:
                    QueueClass.QueueItem(() => Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.FormLoose, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                case OrderType.ArrangementCircular:
                    QueueClass.QueueItem(() => Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.FormCircle, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                case OrderType.ArrangementSchiltron:
                    QueueClass.QueueItem(() => Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.FormSquare, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                case OrderType.ArrangementVee:
                    QueueClass.QueueItem(() => Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.FormSkein, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                case OrderType.ArrangementColumn:
                    QueueClass.QueueItem(() => Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.FormColumn, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                case OrderType.ArrangementScatter:
                    QueueClass.QueueItem(() => Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.FormScatter, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                case OrderType.HoldFire:
                    QueueClass.QueueItem(() => Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.HoldFire, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                case OrderType.FireAtWill:
                    QueueClass.QueueItem(() => Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.FireAtWill, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                case OrderType.Mount:
                    QueueClass.QueueItem(() => Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.Mount, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                case OrderType.Dismount:
                    QueueClass.QueueItem(() => Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.Dismount, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                case OrderType.AIControlOn:
                    QueueClass.QueueItem(() => Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.CommandDelegate, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                case OrderType.AIControlOff:
                    QueueClass.QueueItem(() => Mission.Current.MainAgent.MakeVoice(SkinVoiceManager.VoiceType.CommandUndelegate, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction));
                    return false;
                default:
                    return false;
            }


            return false;
        }
    }
}

