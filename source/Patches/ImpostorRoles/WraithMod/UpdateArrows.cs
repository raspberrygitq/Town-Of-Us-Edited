using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.ImpostorRoles.WraithMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class UpdateArrows
    {
        public static void Postfix(PlayerControl __instance)
        {
            foreach (var role in Role.AllRoles.Where(x => x.RoleType == RoleEnum.Wraith))
            {
                var wraith = (Wraith)role;
                if (PlayerControl.LocalPlayer.Data.IsDead || wraith.Caught)
                {
                    wraith.Arrows.DestroyAll();
                }

                foreach (var arrow in wraith.Arrows) arrow.target = wraith.Player.transform.position;
            }
        }
    }
}