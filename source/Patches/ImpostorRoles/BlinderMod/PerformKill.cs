using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.BlinderMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Blinder);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Blinder>(PlayerControl.LocalPlayer);
            var blindButton = role.BlindButton;
            if (__instance == blindButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.ClosestPlayer == null) return false;
                role.TimeRemaining = CustomGameOptions.BlindDuration;
                role.Blind();
                role.BlindedPlayer = role.ClosestPlayer;
                Utils.Rpc(CustomRPC.Blind, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
                return false;
            }

            return true;
        }
    }
}