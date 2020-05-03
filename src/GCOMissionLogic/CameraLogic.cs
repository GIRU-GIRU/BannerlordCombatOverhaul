using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Screen;

namespace GCO.GCOMissionLogic
{ 
    internal class CameraLogic : MissionLogic
    {
        private const float _maxDistance = 200f;
        private const float _minDistance = 25f;
        public float _distanceToAdd { get; private set; }
        private float _maxHeight { get; set; }
        private bool _MissionSizeNotSet = true;


        public override void OnMissionTick(float dt)
        {
            if (_MissionSizeNotSet)
            {
                if (Mission.AllAgents.Count > 0)
                {
                    DetermineBattleSize();
                    _MissionSizeNotSet = false;
                }
                else
                {
                    _MissionSizeNotSet = true;
                }
            }
        }

        internal bool ShouldOccur()
        {
            bool notStartup = Mission.Mode != TaleWorlds.Core.MissionMode.StartUp;
            bool notConvo = Mission.Mode != TaleWorlds.Core.MissionMode.Conversation;
            bool notDuel = Mission.Mode != TaleWorlds.Core.MissionMode.Duel;
            bool notHideout = Mission.Mode != TaleWorlds.Core.MissionMode.Stealth;

            return notStartup && notConvo && notDuel && notHideout;
        }

        public float HeightOffset(ref MissionScreen __instance)
        {
            var bannerPosition = __instance.GetOrderFlagPosition();
            var playerPosition = __instance.Mission.MainAgent.Position;
            var bannerDistance = playerPosition.Distance(bannerPosition);

            return bannerDistance < _minDistance ? 0f : CalculateOffset(bannerDistance);
        }

        private void DetermineBattleSize()
        {
            var playerAgent = Mission.Agents.Where(x => x.IsPlayerControlled).FirstOrDefault();
            var playerTeamCount = Mission.Agents.Where(x => x.Team == playerAgent.Team).Count();

            if (playerTeamCount < 70)
            {
                _maxHeight = 1.8f;
                _distanceToAdd = 2f;
            }

            if (playerTeamCount >= 70)
            {
                _maxHeight = 5.5f;
                _distanceToAdd = 2f;
            }

            if (playerTeamCount >= 130)
            {
                _maxHeight = 8.5f;
                _distanceToAdd = 5f;
            }
        }

        private float CalculateOffset(float bannerDistance)
        {
            // var result = (bannerDistance / _maxDistance) * 10f;
            // return _maxHeight < result ? _maxHeight : result;

            return _maxHeight;
        }
    }
}


//internal static class Audio
//{
//    private static SoundEvent _SelectOrderCameraSwoosh = null;
//    public static void CreateSelectOrderCameraSwoosh()
//    {
//        //if (_SelectOrderCameraSwoosh == null)
//        //{
//        //    string soundLocation = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Audio\\WindSwooshCamera.mp3");

//        //    if (File.Exists(soundLocation))
//        //    {
//        //        _SelectOrderCameraSwoosh = SoundEvent.CreateEventFromExternalFile("SelectOrderCameraSwoosh", soundLocation, Mission.Current.Scene);
//        //    }

//        //}
//    }

//    private static void CreateSelectOrderCameraSwooshNew(ref MissionScreen __instance)
//    {
//        if (_SelectOrderCameraSwoosh == null)
//        {
//            string soundLocation = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Audio\\SoundSwooshOgg.ogg");

//            if (File.Exists(soundLocation))
//            {
//                _SelectOrderCameraSwoosh = SoundEvent.CreateEventFromExternalFile("SelectOrderCameraSwoosh", soundLocation, __instance.Mission.Scene);
//            }

//        }
//    }

//    public static void PlaySelectOrderCameraSwoosh(ref MissionScreen __instance)
//    {


//        //SoundEvent.PlaySound2D("event:/ui/mission/deploy");

//        if (_SelectOrderCameraSwoosh == null)
//        {
//            CreateSelectOrderCameraSwooshNew(ref __instance);

//            var test = SoundEvent.GetEventIdFromString("SelectOrderCameraSwoosh");
//        }


//        if (!_SelectOrderCameraSwoosh.IsPlaying())
//        {
//            _SelectOrderCameraSwoosh.Play();
//        }

//    }
//}



//private static readonly string[] orderKeys = new string[]
//{
//    "SelectOrder1",
//    "SelectOrder2",
//    "SelectOrder3",
//    "SelectOrder4",
//    "SelectOrder5",
//    "SelectOrder6",
//    "SelectOrder7",
//    "SelectOrder8",
//};

//public static bool IsOrderKeyPressed(ref MissionScreen __instance)
//{
//    for (int i = 0; i < orderKeys.Count(); i++)
//    {
//        if (__instance.InputManager.IsHotKeyDown(orderKeys[i])) return true;

//    }

//    return false;
//}
