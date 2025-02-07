using Discord;
using HarmonyLib;
namespace TownOfUs.Patches
{
    [HarmonyPatch]
    public class DiscordStatus
    {
        [HarmonyPatch(typeof(ActivityManager), nameof(ActivityManager.UpdateActivity))]
        [HarmonyPrefix]
        public static void Prefix([HarmonyArgument(0)] Activity activity)
        {
            if (activity == null) return;

            var details = $"Town of Us Edited v{TownOfUs.VersionString}";
            if (TownOfUs.VersionTag != "<color=#00F0FF></color>") details += " Dev 5";
            activity.Details = details;
        }
    }
}
