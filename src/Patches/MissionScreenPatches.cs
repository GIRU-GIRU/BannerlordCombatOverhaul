using GCO.GCOToolbox;
using GCO.GCOMissionLogic;
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
                            cameraLogic.ApplyCamDistance(ref ____cameraSpecialTargetDistanceToAdd);
                            cameraLogic.ApplyCamHeight(ref __instance, ref ____cameraSpecialTargetPositionToAdd);
                        }
                        else
                        {
                            cameraLogic.ApplyCamReturnSpeed(ref ____cameraSpecialTargetPositionToAdd,
                                                                        ref ____cameraSpecialTargetDistanceToAdd,
                                                                             ref ____cameraSpecialTargetAddedElevation);
                        }
                        
                    }
                }
            }
        }


    }
}

