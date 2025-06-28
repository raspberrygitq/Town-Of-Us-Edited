﻿using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;
using AmongUs.GameOptions;

namespace TownOfUsEdited.CrewmateRoles.SeerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (CustomGameOptions.GameMode != GameMode.Werewolf)
            {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Seer);
            if (!flag) return true;
            var role = Role.GetRole<Seer>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            if (role.Cooldown > 0) return false;
            if (!__instance.enabled) return false;
            var maxDistance = LegacyGameOptions.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                role.Investigated.Add(role.ClosestPlayer.PlayerId);
            }
            if (interact[0] == true)
            {
                role.Cooldown = CustomGameOptions.SeerCd;
                return false;
            }
            else if (interact[1] == true)
            {
                role.Cooldown = CustomGameOptions.TempSaveCdReset;
                return false;
            }
            else if (interact[3] == true) return false;
            }
            else
            {
                var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Seer);
                if (!flag) return true;
                var role = Role.GetRole<Seer>(PlayerControl.LocalPlayer);
                if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
                if (!__instance.enabled) return false;
                var maxDistance = LegacyGameOptions.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
                if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                if (role.ClosestPlayer == null) return false;
                role.Revealed.Add(role.ClosestPlayer.PlayerId);
                role.UsedReveal = true;
                return false;
            }
            return false;
        }
    }
}
