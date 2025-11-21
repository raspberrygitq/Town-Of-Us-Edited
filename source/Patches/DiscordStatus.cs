using Discord;
using HarmonyLib;
namespace TownOfUsEdited.Patches
{
    [HarmonyPatch]
    public class DiscordStatus
    {
        [HarmonyPatch(typeof(ActivityManager), nameof(ActivityManager.UpdateActivity))]
        [HarmonyPrefix]
        public static void Prefix([HarmonyArgument(0)] Activity activity)
        {
            activity.Details = $"Town of Us Edited v{TownOfUsEdited.VersionString}";
        }
    }
}
