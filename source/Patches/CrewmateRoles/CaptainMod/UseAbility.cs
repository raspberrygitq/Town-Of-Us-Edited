using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.CaptainMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class UseAbility
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Captain))
            {
                var cap = (Captain)role;
                if (cap.Zooming)
                    cap.ZoomAbility();
                else if (cap.ZoomEnabled) cap.UnZoomAbility();
            }
        }
    }
}