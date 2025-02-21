using HarmonyLib;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.WerewolfRoles.GuardMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ShowShield
    {
        public static Color ProtectedColor = Color.cyan;

        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Guard))
            {
                var guard = (Guard) role;

                var player = guard.ProtectedPlayer;
                if (PlayerControl.LocalPlayer != guard.Player) continue;
                if (player == null) continue;

                player.myRend().material.SetColor("_VisorColor", ProtectedColor);
                player.myRend().material.SetFloat("_Outline", 1f);
                player.myRend().material.SetColor("_OutlineColor", ProtectedColor);
            }
        }
    }
}