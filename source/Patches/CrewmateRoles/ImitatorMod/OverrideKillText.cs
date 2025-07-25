﻿using HarmonyLib;

namespace TownOfUsEdited.CrewmateRoles.ImitatorMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class OverrideKillText
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (StartImitate.ImitatingPlayers.Count == 0) return;
            if (!StartImitate.ImitatingPlayers.Contains(PlayerControl.LocalPlayer.PlayerId)) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff) && !PlayerControl.LocalPlayer.Is(RoleEnum.Hunter)
            && !PlayerControl.LocalPlayer.Is(RoleEnum.Knight) && !PlayerControl.LocalPlayer.Is(RoleEnum.Fighter)) __instance.KillButton.OverrideText("");
            return;
        }
    }
}