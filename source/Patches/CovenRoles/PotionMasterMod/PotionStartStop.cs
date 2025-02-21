using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CovenRoles.PotionMasterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class PotionStartStop
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.PotionMaster))
            {
                var pm = (PotionMaster) role;
                if (pm.UsingPotion)
                    pm.UsePotion();
                else if (pm.Enabled) pm.StopPotion();
            }
        }
    }
}