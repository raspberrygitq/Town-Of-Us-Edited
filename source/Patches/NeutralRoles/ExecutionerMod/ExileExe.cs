using HarmonyLib;
using System;
using TownOfUsEdited.CrewmateRoles.AltruistMod;
using TownOfUsEdited.Patches;
using TownOfUsEdited.Roles;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUsEdited.NeutraleRoles.ExecutionerMod
{
    [HarmonyPatch(typeof(AirshipExileController._WrapUpAndSpawn_d__11), nameof(AirshipExileController._WrapUpAndSpawn_d__11.MoveNext))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController._WrapUpAndSpawn_d__11 __instance) => ExileExe.ExileControllerPostfix(__instance.__4__this);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class ExileExe
    {
        public static void ExileControllerPostfix(ExileController __instance)
        {
            var exiled = __instance.initData?.networkedPlayer;
            if (exiled == null) return;
            var player = exiled.Object;
            
            foreach (var role in Role.GetRoles(RoleEnum.Troll))
            {
                if (((Troll)role).TrolledPlayer != null && player.PlayerId == ((Troll)role).TrolledPlayer.PlayerId && ((Troll)role).UsedTroll)
                {
                    return;
                }
            }

            foreach (var role in Role.GetRoles(RoleEnum.Executioner))
                if (player.PlayerId == ((Executioner)role).target.PlayerId && !CustomGameOptions.NeutralEvilWinEndsGame)
                {
                    KillButtonTarget.DontRevive = role.Player.PlayerId;
                    role.Player.Exiled();
                    role.DeathReason = DeathReasons.Victorious;
                }
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