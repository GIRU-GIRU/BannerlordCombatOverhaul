using GCO.Utility;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace GCO.GCOMissionLogic
{
    internal static class OrderControllerLogic
    {
        internal static bool IsFormationSelectable(Formation formation, Agent selectorAgent)
        {
            return (selectorAgent == null || formation.PlayerOwner == selectorAgent) && formation.CountOfUnits > 0;
        }

        internal static readonly ActionIndexCache[] CheerActions = new ActionIndexCache[]
        {
            ActionIndexCache.Create("act_cheer_1"),
            ActionIndexCache.Create("act_cheer_2"),
            ActionIndexCache.Create("act_cheer_3"),
            ActionIndexCache.Create("act_cheer_4")
        };

        internal static bool AfterSetOrderMakeVoice(OrderType orderType, Agent agent)
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
                    VoiceCommandQueue.QueueItem("Move", 800f);
                    return false;
                case OrderType.Charge:
                case OrderType.ChargeWithTarget:
                    VoiceCommandQueue.QueueItem("Charge", 800f);
                    return false;
                case OrderType.StandYourGround:
                    VoiceCommandQueue.QueueItem("Stop", 800f);
                    return false;
                case OrderType.FollowMe:
                    VoiceCommandQueue.QueueItem("Follow", 800f);
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
                    VoiceCommandQueue.QueueItem("Retreat", 1200f);
                    return false;
                case OrderType.AdvanceTenPaces:
                case OrderType.Advance:
                    VoiceCommandQueue.QueueItem("Advance", 1200f);
                    return false;
                case OrderType.FallBackTenPaces:
                case OrderType.FallBack:
                    VoiceCommandQueue.QueueItem("FallBack", 1200f);
                    return false;
                case OrderType.LookAtEnemy:
                    VoiceCommandQueue.QueueItem("FaceEnemy", 1200f);
                    return false;
                case OrderType.LookAtDirection:
                    VoiceCommandQueue.QueueItem("FaceDirection", 1200f);
                    break;
                case OrderType.ArrangementLine:
                    VoiceCommandQueue.QueueItem("FormLine", 1200f);
                    return false;
                case OrderType.ArrangementCloseOrder:
                    VoiceCommandQueue.QueueItem("FormShieldWall", 1700f);
                    return false;
                case OrderType.ArrangementLoose:
                    VoiceCommandQueue.QueueItem("FormLoose", 1200f);
                    return false;
                case OrderType.ArrangementCircular:
                    VoiceCommandQueue.QueueItem("FormCircle", 1200f);
                    return false;
                case OrderType.ArrangementSchiltron:
                    VoiceCommandQueue.QueueItem("FormSquare", 1200f);
                    return false;
                case OrderType.ArrangementVee:
                    VoiceCommandQueue.QueueItem("FormSkein", 1200f);
                    return false;
                case OrderType.ArrangementColumn:
                    VoiceCommandQueue.QueueItem("FormColumn", 1200f);
                    return false;
                case OrderType.ArrangementScatter:
                    VoiceCommandQueue.QueueItem("FormScatter", 1200f);
                    return false;
                case OrderType.HoldFire:
                    VoiceCommandQueue.QueueItem("HoldFire", 1200f);
                    return false;
                case OrderType.FireAtWill:
                    VoiceCommandQueue.QueueItem("FireAtWill", 1200f);
                    return false;
                case OrderType.Mount:
                    VoiceCommandQueue.QueueItem("Mount", 1200f);
                    return false;
                case OrderType.Dismount:
                    VoiceCommandQueue.QueueItem("Dismount", 1200f);
                    return false;
                case OrderType.AIControlOn:
                    VoiceCommandQueue.QueueItem("CommandDelegate", 1200f);
                    return false;
                case OrderType.AIControlOff:
                    VoiceCommandQueue.QueueItem("CommandUndelegate", 1200f);
                    return false;
                default:
                    return false;
            }

            return false;
        }
    }
}
