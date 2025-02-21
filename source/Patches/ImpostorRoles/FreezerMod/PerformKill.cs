using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.ImpostorRoles.FreezerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Freezer);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Freezer>(PlayerControl.LocalPlayer);
            var freezeButton = role.FreezeButton;
            if (__instance == freezeButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.ClosestPlayer == null) return false;
                role.TimeRemaining = CustomGameOptions.FreezeDuration;
                role.Freeze();
                role.FrozenPlayer = role.ClosestPlayer;
                Utils.Rpc(CustomRPC.Freeze, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
                return false;
            }

            return true;
        }
    }
}