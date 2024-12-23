using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.HelperMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Helper);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Helper>(PlayerControl.LocalPlayer);
            var alertButton = DestroyableSingleton<HudManager>.Instance.KillButton;
            if (__instance == alertButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.ClosestPlayer == null) return false;
                role.TimeRemaining = CustomGameOptions.HelperDuration;
                role.Alert();
                role.AlertedPlayer = role.ClosestPlayer;
                Utils.Rpc(CustomRPC.HelperAlert, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
                return false;
            }

            return true;
        }
    }
}