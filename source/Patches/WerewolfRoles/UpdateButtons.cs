using HarmonyLib;
using UnityEngine;

namespace TownOfUsEdited.WerewolfRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class UpdateSabotage
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            if (CustomGameOptions.GameMode != GameMode.Werewolf) return;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(Faction.Impostors)) return;
            __instance.KillButton.transform.localPosition = new Vector3(0f, 1f, 0f);
        }
    }
}