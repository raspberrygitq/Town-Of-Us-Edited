using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using HarmonyLib;
using AmongUs.GameOptions;

namespace TownOfUs
{
    [HarmonyPatch]

    internal sealed class Hauntpatch
    {
        [HarmonyPatch(typeof(HauntMenuMinigame), nameof(HauntMenuMinigame.SetFilterText))]
        [HarmonyPrefix]

        public static bool Prefix(HauntMenuMinigame __instance)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return true;
            var role = Role.GetRole(__instance.HauntTarget);
            var modifier = Modifier.GetModifier(__instance.HauntTarget);

            if (CustomGameOptions.GameMode == GameMode.Werewolf)
            {
                __instance.FilterText.text = "Village Member";
                return false;
            }

            __instance.FilterText.text = modifier != null ? $"{role.Name} - {modifier.Name}"
                                                          : $"{role.Name}";

            // Avoid NullReferenceException i guess
            if (modifier != null)
            {
                if (role.Faction == Faction.Madmates && modifier.Name != "Madmate")
                {
                    __instance.FilterText.text += "<color=#FF0000>(Madmate)</color>";
                }
            }
            else
            {
                if (role.Faction == Faction.Madmates)
                {
                    __instance.FilterText.text += "<color=#FF0000>(Madmate)</color>";
                }
            }
            return false;
        }
    }
}