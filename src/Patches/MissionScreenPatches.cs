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
using GCO.ReversePatches;
using GCO.Utility;

namespace GCO.Patches
{
    internal static class MissionScreenPatches
    {
        private static bool ShoulderCamExists = false;

        [HarmonyBefore("xorberax.shouldercam")]
        internal static bool UpdateCameraPrefix(ref MissionScreen __instance, ref float ____cameraSpecialTargetAddedBearing, ref Vec3 ____cameraSpecialTargetPositionToAdd, ref float ____cameraSpecialTargetDistanceToAdd, ref float ____cameraSpecialTargetAddedElevation, ref float dt)
        {
            var cameraLogic = __instance.Mission.GetMissionBehaviour<CameraLogic>();
            if (cameraLogic != null)
            {
                if (cameraLogic.ShouldOccur())
                {
                    if (__instance.OrderFlag != null && __instance.OrderFlag.IsVisible)
                    {

                        cameraLogic.ApplyCamDistance(ref ____cameraSpecialTargetDistanceToAdd, __instance.CameraResultDistanceToTarget);
                        cameraLogic.ApplyCamHeight(ref __instance, ref ____cameraSpecialTargetPositionToAdd);

                        MissionScreenReversePatches.UpdateCamera(__instance, dt);

                        return false;

                    }
                    else if (!CompatibilityCheck.XorbShoulderCamDetected)
                    {
                        cameraLogic.ApplyCamReturnSpeed(ref ____cameraSpecialTargetPositionToAdd,
                                                                    ref ____cameraSpecialTargetDistanceToAdd,
                                                                        ref ____cameraSpecialTargetAddedElevation);
                    }

                }
            }


            return true;
        }



    }


}


