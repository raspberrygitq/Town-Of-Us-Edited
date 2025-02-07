using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.ManipulatorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class ControlUpdate
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Manipulator))
            {
                var manip = (Manipulator) role;
                if (manip.UsingManipulation)
                    manip.StartManipulation();
                else if (manip.Enabled) manip.StopManipulation();
            }
        }
    }
}