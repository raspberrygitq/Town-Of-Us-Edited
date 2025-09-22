using AmongUs.GameOptions;
using HarmonyLib;
using System.Linq;

namespace TownOfUsEdited.Patches;

internal static class GenericPatches
{
    [HarmonyPatch(typeof(NormalGameOptionsV09), nameof(NormalGameOptionsV09.AreInvalid))]
    public static class InvalidOptionsPatches
    {
        public static bool Prefix(NormalGameOptionsV09 __instance, [HarmonyArgument(0)] int maxExpectedPlayers)
        {
            return __instance.MaxPlayers > maxExpectedPlayers ||
                   __instance.NumImpostors < 1 ||
                   __instance.NumImpostors + 1 > maxExpectedPlayers / 2 ||
                   __instance.KillDistance is < 0 or > 2 ||
                   __instance.PlayerSpeedMod is <= 0f or > 3f;
        }
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public static class GameStartManagerUpdatePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CreateGameOptions), nameof(CreateGameOptions.Show))]
        public static void CreateGameOptionsShowPostfix(CreateGameOptions __instance)
        {
            var numberOption = __instance.gameObject.GetComponentInChildren<NumberOption>(true);
            if (numberOption != null)
            {
                numberOption.ValidRange.max = TownOfUsEdited.MaxPlayers;
            }
        }
    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Initialize))]
    public static class GameOptionsMenu_Initialize
    {
        public static void Postfix(GameOptionsMenu __instance)
        {
            var numberOptions = __instance.GetComponentsInChildren<NumberOption>();

            var impostorsOption = numberOptions.FirstOrDefault(o => o.Title == StringNames.GameNumImpostors);
            if (impostorsOption != null)
            {
                impostorsOption.ValidRange = new FloatRange(1, TownOfUsEdited.MaxImpostors);
            }

        }
    }
}
