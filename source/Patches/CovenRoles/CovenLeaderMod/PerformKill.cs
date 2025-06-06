using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.Patches.CovenRoles.CovenLeaderMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.CovenLeader))
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            var role = Role.GetRole<CovenLeader>(PlayerControl.LocalPlayer);

            if (__instance == role.RecruitButton)
            {
                if (role.KillCooldown > 0)
                return false;

                if (role.ClosestPlayer == null)
                return false;

                role.Recruit(role.ClosestPlayer);
            }

            return false;
        }
    }
}