using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.Patches.CovenRoles.VoodooMasterMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.VoodooMaster))
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            var role = Role.GetRole<VoodooMaster>(PlayerControl.LocalPlayer);
            
            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            if (__instance == role.VoodooButton)
            {
                if (role.KillCooldown > 0)
                return false;

                if (role.ClosestPlayer == null)
                return false;

                role.VoodooPlayer = role.ClosestPlayer;
                Utils.Rpc(CustomRPC.SetVoodooPlayer, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
                role.KillCooldown = CustomGameOptions.CovenKCD;
            }

            return false;
        }
    }
}