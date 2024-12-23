using HarmonyLib;
using TownOfUs.Roles;
using TownOfUs.Modifiers.UnderdogMod;
using UnityEngine;

namespace TownOfUs.Patches.ImpostorRoles.ShooterMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Shooter);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Shooter>(PlayerControl.LocalPlayer);

            var killButton = DestroyableSingleton<HudManager>.Instance.KillButton;
  
            if (__instance == role.StoreButton) 
            {
                if (!role.ButtonUsable) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (killButton.isCoolingDown) return false;
                role.UsesLeft++;
                Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                return false;
            }

            return false;
        }
    }
}