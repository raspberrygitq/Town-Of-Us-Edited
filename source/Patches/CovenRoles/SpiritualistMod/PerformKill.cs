using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.Patches.CovenRoles.SpiritualistMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Spiritualist))
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            var role = Role.GetRole<Spiritualist>(PlayerControl.LocalPlayer);
            
            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            if (__instance == role.ControlButton)
            {
                if (role.KillCooldown > 0)
                return false;

                if (role.ClosestPlayer == null)
                return false;

                role.Control(role.ClosestPlayer);
                Utils.Rpc(CustomRPC.SpiritualistControl, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
            }

            return false;
        }
    }
}