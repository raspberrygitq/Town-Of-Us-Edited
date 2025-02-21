using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.ImpostorRoles.BlinderMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class GuardUnguard
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Blinder))
            {
                var blinder = (Blinder) role;
                if (blinder.Blinding)
                    blinder.Blind();
                else if (blinder.Enabled) blinder.UnBlind();
            }
        }
    }
}