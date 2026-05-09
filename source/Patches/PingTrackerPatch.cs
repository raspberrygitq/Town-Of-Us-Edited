using HarmonyLib;

namespace TownOfUsEdited.Patches;

[HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
internal static class PingTrackerPatch
{
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    public static void Postfix(PingTracker __instance)
    {
        var extraText = $"<align=center><size=60%><space=3em>Region: <color=#C96DFF>{Utils.GetRegionName()}</color></size></align>";
        __instance.text.text += $"\r\n{extraText}";
    }
}