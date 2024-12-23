using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.HelperMod
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
    public static class PlayerPhysics_FixedUpdate
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Helper))
            {
                var helper = (Helper)role;
                if (__instance.myPlayer == helper.AlertedPlayer && helper.OnAlert)
                {
                    if (__instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove)
                    {
                        __instance.body.velocity = __instance.body.velocity * CustomGameOptions.HelperSpeed;
                    }
                }
            }
        }
    }
}