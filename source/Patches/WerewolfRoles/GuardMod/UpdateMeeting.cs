using HarmonyLib;
using System;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsEdited.Patches;
using TownOfUsEdited.Roles;
using TownOfUsEdited.Extensions;

namespace TownOfUsEdited.CrewmateRoles.GuardMod
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => MeetingExiledEnd.ExileControllerPostfix(__instance);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    internal class MeetingExiledEnd
    {
        public static void ExileControllerPostfix(ExileController __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Guard)) return;
            var role = Role.GetRole<Guard>(PlayerControl.LocalPlayer);

            role.ProtectedPlayer.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
            role.ProtectedPlayer.myRend().material.SetFloat("_Outline", 0f);
            role.UsedProtect = false;
            role.ProtectedPlayer = null;
            Utils.Rpc(CustomRPC.UpdateGuard, PlayerControl.LocalPlayer.PlayerId);
        }
        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance?.currentNormalGameOptions?.MapId != 6) return;
            if (obj.name?.Contains("ExileCutscene") == true) ExileControllerPostfix(ExileControllerPatch.lastExiled);
        }
    }
}