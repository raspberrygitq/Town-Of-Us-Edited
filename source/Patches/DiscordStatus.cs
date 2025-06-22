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
            if (activity == null) return;

            var details = $"Town of Us Edited v{TownOfUsEdited.CompilationString}";
            if (TownOfUsEdited.VersionTag != "<color=#00F0FF></color>") details += " Dev 15";
            activity.Details = details;
        }
    }
}
