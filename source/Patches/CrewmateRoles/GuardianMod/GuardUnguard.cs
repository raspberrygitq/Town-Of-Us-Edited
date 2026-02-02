using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.GuardianMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class GuardUnguard
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Guardian))
            {
                var guardian = (Guardian)role;
                if (guardian.Guarding)
                    guardian.Guard();
                else if (guardian.Enabled) guardian.UnGuard();
            }
        }
    }
}