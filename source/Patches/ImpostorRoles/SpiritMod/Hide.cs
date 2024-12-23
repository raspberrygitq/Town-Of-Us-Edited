using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.SpiritMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class Hide
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Spirit))
            {
                var spirit = (Spirit) role;
                if (spirit.Player.Data.Disconnected) return;
                var caught = spirit.Caught;
                if (!caught)
                {
                    spirit.Fade();
                }
                else if (spirit.Faded)
                {
                    Utils.Unmorph(spirit.Player);
                    spirit.Player.myRend().color = Color.white;
                    spirit.Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
                    spirit.Faded = false;
                    spirit.Player.MyPhysics.ResetMoveState();
                }
            }
        }
    }
}