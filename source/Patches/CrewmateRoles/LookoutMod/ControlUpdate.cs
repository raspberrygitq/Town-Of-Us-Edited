using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.LookoutMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class ControlUpdate
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Lookout))
            {
                var lookout = (Lookout) role;
                if (lookout.UsingWatch)
                    lookout.StartWatching();
                else if (lookout.Enabled) lookout.StopWatching();
            }
        }
    }
}