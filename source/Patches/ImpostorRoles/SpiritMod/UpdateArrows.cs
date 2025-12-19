using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.ImpostorRoles.SpiritMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class UpdateArrows
    {
        public static void Postfix(PlayerControl __instance)
        {
            foreach (var role in Role.AllRoles.Where(x => x.RoleType == RoleEnum.Spirit))
            {
                var spirit = (Spirit)role;
                if (PlayerControl.LocalPlayer.Data.IsDead || spirit.Caught)
                {
                    spirit.Arrows.DestroyAll();
                }

                foreach (var arrow in spirit.Arrows) arrow.target = spirit.Player.transform.position;
            }
        }
    }
}