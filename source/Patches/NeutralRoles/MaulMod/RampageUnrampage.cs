using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.NeutralRoles.MaulMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class RampageUnrampage
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Maul))
            {
                var werewolf = (Maul) role;
                if (werewolf.Rampaged)
                    werewolf.Rampage();
                else if (werewolf.Enabled) werewolf.Unrampage();
            }
        }
    }
}