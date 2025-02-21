using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.HelperMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class AlertUnalert
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Helper))
            {
                var helper = (Helper) role;
                if (helper.OnAlert)
                    helper.Alert();
                else if (helper.Enabled) helper.UnAlert();
            }
        }
    }
}