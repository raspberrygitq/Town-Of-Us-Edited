using AmongUs.GameOptions;
using HarmonyLib;
using TMPro;
using TownOfUsEdited.Roles;
using TownOfUsEdited.Roles.Modifiers;

namespace TownOfUsEdited
{
    [HarmonyPatch]

    internal sealed class Hauntpatch
    {
        [HarmonyPatch(typeof(HauntMenuMinigame), nameof(HauntMenuMinigame.SetHauntTarget))]
        [HarmonyPrefix]

        public static bool Prefix(HauntMenuMinigame __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return true;
            if (target == null)
            {
                __instance.HauntTarget = null;
                __instance.NameText.text = "";
                __instance.FilterText.text = "";
                __instance.HauntingText.enabled = false;
                return false;
            }
            __instance.HauntTarget = target;
            __instance.HauntingText.enabled = true;
            TMP_Text nameText = __instance.NameText;
            NetworkedPlayerInfo data = target.Data;
            nameText.text = ((data != null) ? data.GetPlayerName(PlayerOutfitType.Default) : null);
            var role = Role.GetRole(__instance.HauntTarget);
            var modifiers = Modifier.GetModifiers(__instance.HauntTarget);

            if (modifiers.Length == 0) __instance.FilterText.text = role.Name;
            else
            {
                string modifierText = " (";
                foreach (var modifier in modifiers)
                {
                    if (modifierText != " (") modifierText += ", ";
                    modifierText += modifier.Name;
                }
                modifierText += ")";
                __instance.FilterText.text = role.Name + modifierText;
            }

            return false;
        }
    }
}