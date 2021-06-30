using GCO.Utility;
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
        private TeamSizeEnum _PlayerTeamSize;

        private float _distanceToAdd;
        private float _maxHeight;
        private float _minDistance;
        private bool _battleSizeNotDetermined = true;
        private float _camOffset = 0.2f;

        internal enum TeamSizeEnum
        {
            Small = 0,
            Medium = 1,
            Large = 2,
        }


        public override void OnMissionTick(float dt)
        {
            if (_battleSizeNotDetermined)
            {
                if (Mission.AllAgents != null && Mission.AllAgents.Count > 0)
                {
                    DetermineBattleSize();
                    CompatibilityCheck.CheckForShoulderCam();
                    _battleSizeNotDetermined = false;
                }
                else
                {
                    _battleSizeNotDetermined = true;
                }
            }
        }

        internal void ApplyCamReturnSpeed(ref Vec3 cameraSpecialTargetPositionToAdd, ref float cameraSpecialTargetDistanceToAdd, ref float cameraSpecialTargetAddedElevation)
        {
            switch (_PlayerTeamSize)
            {
                case TeamSizeEnum.Small:
                    CalculateCameraSpeedSmall(ref cameraSpecialTargetPositionToAdd, ref cameraSpecialTargetDistanceToAdd, ref cameraSpecialTargetAddedElevation);
                    break;
                case TeamSizeEnum.Medium:
                    CalculateCameraSpeedMedium(ref cameraSpecialTargetPositionToAdd, ref cameraSpecialTargetDistanceToAdd, ref cameraSpecialTargetAddedElevation);
                    break;
                case TeamSizeEnum.Large:
                    CalculateCameraSpeedLarge(ref cameraSpecialTargetPositionToAdd, ref cameraSpecialTargetDistanceToAdd, ref cameraSpecialTargetAddedElevation);
                    break;
                default:
                    CalculateCameraSpeedSmall(ref cameraSpecialTargetPositionToAdd, ref cameraSpecialTargetDistanceToAdd, ref cameraSpecialTargetAddedElevation);
                    break;
            }
        }

        internal void ApplyCamHeight(ref MissionScreen __instance, ref Vec3 cameraSpecialTargetPositionToAdd)
        {

            cameraSpecialTargetPositionToAdd = new Vec3 { z = _maxHeight };
        }

        internal bool ShouldOccur()
        {
            bool notStartup = Mission.Mode != TaleWorlds.Core.MissionMode.StartUp;
            bool notConvo = Mission.Mode != TaleWorlds.Core.MissionMode.Conversation;
            bool notDuel = Mission.Mode != TaleWorlds.Core.MissionMode.Duel;
            bool notHideout = Mission.Mode != TaleWorlds.Core.MissionMode.Stealth;

            return notStartup && notConvo && notDuel && notHideout;
        }

        internal void ApplyCamDistance(ref float ____cameraSpecialTargetDistanceToAdd, float currentCameraDistance)
        {
            ____cameraSpecialTargetDistanceToAdd = _distanceToAdd - currentCameraDistance;
        }

        private void CalcHeightOffset(ref Vec3 cameraSpecialTargetPositionToAdd)
        {
            var currentZ = cameraSpecialTargetPositionToAdd.z;
            if (currentZ == 0)
            {
                cameraSpecialTargetPositionToAdd = new Vec3 { z = Math.Min(currentZ + 5f, _maxHeight) };
            }
        }

        private void CalculateCameraSpeedLarge(ref Vec3 cameraSpecialTargetPositionToAdd, ref float cameraSpecialTargetDistanceToAdd, ref float cameraSpecialTargetAddedElevation)
        {
            cameraSpecialTargetPositionToAdd = new Vec3 { z = Math.Max(cameraSpecialTargetPositionToAdd.z - _camOffset, 0) };
            cameraSpecialTargetDistanceToAdd = Math.Max(cameraSpecialTargetDistanceToAdd - _camOffset, 0);
            cameraSpecialTargetAddedElevation = Math.Max(cameraSpecialTargetAddedElevation - _camOffset, 0f);
        }

        private void CalculateCameraSpeedMedium(ref Vec3 cameraSpecialTargetPositionToAdd, ref float cameraSpecialTargetDistanceToAdd, ref float cameraSpecialTargetAddedElevation)
        {
            cameraSpecialTargetPositionToAdd = new Vec3 { z = Math.Max(cameraSpecialTargetPositionToAdd.z - _camOffset, 0) };
            cameraSpecialTargetDistanceToAdd = Math.Max(cameraSpecialTargetDistanceToAdd - _camOffset, 0);
            cameraSpecialTargetAddedElevation = Math.Max(cameraSpecialTargetAddedElevation - _camOffset, 0f);
        }

        private void CalculateCameraSpeedSmall(ref Vec3 cameraSpecialTargetPositionToAdd, ref float cameraSpecialTargetDistanceToAdd, ref float cameraSpecialTargetAddedElevation)
        {
            cameraSpecialTargetPositionToAdd = Vec3.Zero;
            cameraSpecialTargetDistanceToAdd = 0f;
            cameraSpecialTargetAddedElevation = 0f;
        }

        private void DetermineBattleSize()
        {
            if (!Mission || Mission.Agents.Count() == 0) return;

            var playerAgent = Mission.Agents.Where(x => x.IsPlayerControlled).FirstOrDefault();

            if (playerAgent != null)
            {
                var playerTeamCount = Mission.Agents.Count(x => x.Team == playerAgent.Team);

                if (playerTeamCount < 70)
                {
                    _PlayerTeamSize = TeamSizeEnum.Small;
                    _maxHeight = 1.7f;
                    _distanceToAdd = 5.3f;
                    _minDistance = 25f;
                }

                if (playerTeamCount >= 70)
                {
                    _PlayerTeamSize = TeamSizeEnum.Medium;
                    _maxHeight = 3.5f;
                    _distanceToAdd = 9.33f;
                    _minDistance = 20f;
                }

                if (playerTeamCount >= 150)
                {
                    _PlayerTeamSize = TeamSizeEnum.Large;
                    _maxHeight = 8.5f;
                    _distanceToAdd = 14.33f;
                    _minDistance = 10f;
                }
            }

        }
    }
}

