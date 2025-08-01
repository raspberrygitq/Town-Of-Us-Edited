﻿using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.ImpostorRoles.NoclipMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Noclip);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Noclip>(PlayerControl.LocalPlayer);
            if (__instance == role.NoclipButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.Cooldown > 0) return false;
                var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                if (!abilityUsed) return false;

                role.NoclipSafePoint = PlayerControl.LocalPlayer.transform.position;
                role.TimeRemaining = CustomGameOptions.NoclipDuration;
                role.WallWalk();
                return false;
            }

            return true;
        }
    }
}
