using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.GuardianMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Guardian);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Guardian>(PlayerControl.LocalPlayer);
            var alertButton = DestroyableSingleton<HudManager>.Instance.KillButton;
            if (__instance == alertButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.ClosestPlayer == null) return false;
                role.TimeRemaining = CustomGameOptions.GuardDuration;
                role.Guard();
                role.ProtectedPlayer = role.ClosestPlayer;
                Utils.Rpc(CustomRPC.Guard, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
                return false;
            }

            return true;
        }
    }
}