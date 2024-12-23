using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.SpiritMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Visible), MethodType.Setter)]
    public static class VisibleOverride
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] ref bool value)
        {
            if (!__instance.Is(RoleEnum.Spirit)) return;
            if (Role.GetRole<Spirit>(__instance).Caught) return;
            value = !__instance.inVent;
        }
    }
}