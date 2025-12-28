using HarmonyLib;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.ImpostorRoles.WraithMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class Hide
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Wraith))
            {
                var wraith = (Wraith) role;
                if (wraith.Player == null || wraith.Player.Data.Disconnected) return;
                var caught = wraith.Caught;
                if (!caught)
                {
                    wraith.Fade();
                }
                else if (wraith.Faded)
                {
                    Utils.Unmorph(wraith.Player);
                    wraith.Player.myRend().color = Color.white;
                    wraith.Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
                    wraith.Faded = false;
                    wraith.Player.Collider.enabled = false;
                    wraith.Player.MyPhysics.ResetMoveState();
                }
            }
        }
    }
}