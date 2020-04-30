using GCO.Features;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Screen;

namespace GCO.Patches
{
    internal static class MissionScreenPatches
    {
        internal static void UpdateCameraPrefix(ref MissionScreen __instance, ref float ____cameraSpecialTargetAddedBearing, ref Vec3 ____cameraSpecialTargetPositionToAdd, ref float ____cameraSpecialTargetDistanceToAdd, ref float ____cameraSpecialTargetAddedElevation)
        {
            var cameraLogic = __instance.Mission.GetMissionBehaviour<CameraLogic>();
            if (cameraLogic != null)
            {
                if (cameraLogic.ShouldOccur())
                {
                    if (__instance.OrderFlag != null)
                    {
                        if (__instance.OrderFlag.IsVisible)
                        {
                            ____cameraSpecialTargetDistanceToAdd = cameraLogic._distanceToAdd;
                            ____cameraSpecialTargetPositionToAdd = new Vec3 { z = cameraLogic.HeightOffset(ref __instance) };
                        }
                        else
                        {
                            ____cameraSpecialTargetPositionToAdd = Vec3.Zero;
                            ____cameraSpecialTargetDistanceToAdd = 0f;
                            ____cameraSpecialTargetAddedElevation = 0f;
                        }
                    }

                }
            }
        }
    }
}

