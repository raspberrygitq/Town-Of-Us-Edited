using System;
using HarmonyLib;
using TownOfUsEdited.Patches;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUsEdited
{    
    //Code from Among Us Salem with authorisation of 50 iq 
    public class DayNightMechanic
    {
        public static int DayCount = 0;
        public static int NightCount = 1;

        [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
        public static class AirshipExileController_WrapUpAndSpawn
        {
            public static void Postfix(AirshipExileController __instance) => TurnNight.ExileControllerPostfix(__instance);
        }

        [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
        public class TurnNight
        {
            public static void ExileControllerPostfix(ExileController __instance)
            {
                if (CustomGameOptions.GameMode != GameMode.Werewolf) return;
                NightCount ++;
                return;
            }

            public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

            [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new Type[] { typeof(GameObject) })]
            public static void Prefix(GameObject obj)
            {
                if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance?.currentNormalGameOptions?.MapId != 6) return;
                if (obj.name?.Contains("ExileCutscene") == true) ExileControllerPostfix(ExileControllerPatch.lastExiled);
            }
        }
        
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        internal class TurnDay
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (CustomGameOptions.GameMode != GameMode.Werewolf) return;
                DayCount ++;
                return;
            }
        }
    }
}