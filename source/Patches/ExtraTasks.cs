using HarmonyLib;
using System;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch(typeof(ShipStatus))]
    public class ShipStatusPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
        public static void Postfix(LogicGameFlowNormal __instance, ref bool __result)
        {
            __result = false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
        public static bool Prefix(ShipStatus __instance)
        {
            var commonTask = Math.Min(__instance.CommonTasks.Count, 4);
            var normalTask = Math.Min(__instance.ShortTasks.Count, 8);
            var longTask = Math.Min(__instance.LongTasks.Count, 4);
            if (GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks > commonTask) GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks = commonTask;
            if (GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks > normalTask) GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks = normalTask;
            if (GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks > longTask) GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks = longTask;
            return true;
        }
    }
}
