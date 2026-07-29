using AmongUs.Data.Player;
using AmongUs.Data.Settings;
using HarmonyLib;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.FileName), MethodType.Getter)]
    [HarmonyPatch(typeof(SettingsData), nameof(SettingsData.FileName), MethodType.Getter)]
    public class SaveManagerPatch
    {
        public static void Postfix(ref string __result)
        {
            __result += "_TOUE";
        }
    }
}