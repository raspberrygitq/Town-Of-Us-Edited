using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.ImpostorRoles.MorphlingMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class MorphUnmorph
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Morphling))
            {
                var morphling = (Morphling) role;
                if (morphling.Morphed)
                    morphling.Morph();
                else if (morphling.Enabled) morphling.Unmorph();
            }
        }
    }
}