using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.NeutralRoles.GlitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class MimicUnmimic
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Glitch))
            {
                var glitch = (Glitch) role;
                if (glitch.IsUsingMimic)
                    Utils.Morph(glitch.Player, glitch.MimicTarget);
                else if (glitch.Enabled)
                {
                    glitch.Enabled = false;
                    Utils.Unmorph(glitch.Player);
                }
            }
        }
    }
}