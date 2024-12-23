using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.AstralMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class TurnGhost
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Astral))
            {
                var astral = (Astral) role;
                if (astral.UsingGhost)
                    astral.TurnGhost(role.Player);
                else if (astral.Enabled) astral.TurnBack(role.Player);
            }
        }
    }
}