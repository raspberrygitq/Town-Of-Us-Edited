using HarmonyLib;
using System;
using System.Linq;
using TownOfUsEdited.Patches;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.NeutralRoles.ArsonistMod
{
    [HarmonyPatch(typeof(AirshipExileController._WrapUpAndSpawn_d__11), nameof(AirshipExileController._WrapUpAndSpawn_d__11.MoveNext))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController._WrapUpAndSpawn_d__11 __instance) => SetLastKillerBool.ExileControllerPostfix(__instance.__4__this);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class SetLastKillerBool
    {

        public static void ExileControllerPostfix(ExileController __instance)
        {
            var exiled = __instance.initData?.networkedPlayer?.Object;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist)) return;
            var alives = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected && !x == PlayerControl.LocalPlayer).ToList();
            foreach (var player in alives)
            {
                if (player.Is(Faction.Impostors) || player.Is(Faction.NeutralKilling) || player.Is(Faction.Coven)) return;
            }
            var role = Role.GetRole<Arsonist>(PlayerControl.LocalPlayer);
            role.LastKiller = true;
            return;
        }

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(UnityEngine.Object), nameof(UnityEngine.Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance?.currentNormalGameOptions?.MapId != 6) return;
            if (obj.name?.Contains("ExileCutscene") == true) ExileControllerPostfix(ExileControllerPatch.lastExiled);
        }
    }
}