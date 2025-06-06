using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.ImpostorRoles.TraitorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Traitor);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Traitor>(PlayerControl.LocalPlayer);
            if (__instance == role.ChangeRoleButton)
            {
                if (!__instance.isActiveAndEnabled) return false;
                SetTraitor.StartChangeRole(PlayerControl.LocalPlayer);
                return false;
            }

            return true;
        }
    }
}