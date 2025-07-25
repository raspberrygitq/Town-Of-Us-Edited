﻿using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.TrapperMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Trapper)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Trapper>(PlayerControl.LocalPlayer);
            if (role.Cooldown > 0) return false;
            if (!__instance.enabled) return false;
            if (!role.ButtonUsable) return false;
            var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
            if (!abilityUsed) return false;
            role.UsesLeft--;
            role.Cooldown = CustomGameOptions.TrapCooldown;
            var pos = PlayerControl.LocalPlayer.transform.position;
            pos.y -= 0.2727f;
            pos.z += 0.001f;
            role.traps.Add(TrapExtentions.CreateTrap(pos));

            return false;
        }
    }
}
