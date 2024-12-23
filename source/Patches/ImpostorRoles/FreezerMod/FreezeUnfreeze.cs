using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.FreezerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class GuardUnguard
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Freezer))
            {
                var freezer = (Freezer) role;
                if (freezer.Freezing)
                    freezer.Freeze();
                else if (freezer.Enabled) freezer.UnFreeze();
            }
        }
    }
}