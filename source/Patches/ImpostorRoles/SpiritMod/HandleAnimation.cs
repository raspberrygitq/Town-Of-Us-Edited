using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.SpiritMod
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
    public class HandleAnimation
    {
        public static void Prefix(PlayerPhysics __instance, [HarmonyArgument(0)] ref bool amDead)
        {
            if (__instance.myPlayer.Is(RoleEnum.Spirit)) amDead = Role.GetRole<Spirit>(__instance.myPlayer).Caught;
        }
    }
}