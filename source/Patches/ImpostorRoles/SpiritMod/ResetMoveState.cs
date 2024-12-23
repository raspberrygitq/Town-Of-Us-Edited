using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.SpiritMod
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ResetMoveState))]
    public class ResetMoveState
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            if (!__instance.myPlayer.Is(RoleEnum.Spirit)) return;

            var role = Role.GetRole<Spirit>(__instance.myPlayer);
            __instance.myPlayer.Collider.enabled = !role.Caught;
        }
    }
}