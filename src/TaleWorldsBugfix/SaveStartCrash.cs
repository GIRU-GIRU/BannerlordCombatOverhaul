//using GCO.ReversePatches;
//using HarmonyLib;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TaleWorlds.CampaignSystem;
//using TaleWorlds.Core;
//using TaleWorlds.SaveSystem.Load;
//using static HarmonyLib.AccessTools;

//namespace GCO.TaleWorldsBugfix
//{
//    [HarmonyPatch]
//    class SaveStartCrash
//    {
//        [HarmonyPatch(typeof(VillageType), "GetProductionPerDay", typeof(ItemCategory))]
//        [HarmonyPrefix]
//        public static bool GetProductionPerDay(ref VillageType __instance, ref float __result, ItemCategory itemCategory)
//        {
//            float num = 0f;
//            var _productions = MissionAccessTools.Get_productions(ref __instance);
//            foreach (ValueTuple<ItemObject, float> valueTuple in _productions)
//            {
//                if (valueTuple.Item1 != null)
//                {
//                    if (valueTuple.Item1.ItemCategory == itemCategory)
//                    {
//                        num += valueTuple.Item2;
//                    }
//                }
//            }
//            __result = num;

//            return false;
//        }
//    }
//}
