using HarmonyLib;
using Reactor.Utilities;
using System.Linq;
using TownOfUs.Patches.NeutralRoles;
using TownOfUs.Roles;
using System;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUs.Patches;

namespace TownOfUs.NeutralRoles.TrollMod
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
            var exiled = __instance.initData.networkedPlayer;
            if (exiled == null) return;
            var player = exiled.Object;

            foreach (var role in Role.GetRoles(RoleEnum.Troll))
            {
                if (((Troll)role).TrolledPlayer != null && player.PlayerId == ((Troll)role).TrolledPlayer.PlayerId && ((Troll)role).UsedTroll == true)
                {
                    ((Troll)role).Wins();
                    Utils.Rpc(CustomRPC.TrollWin, ((Troll)role).Player.PlayerId);

                    if (CustomGameOptions.NeutralEvilWinEndsGame || !CustomGameOptions.TrollHaunts) return;
                    if (PlayerControl.LocalPlayer != player) return;
                    role.PauseEndCrit = true;
                    role.DeathReason = DeathReasons.Won;

                    byte[] toKill = MeetingHud.Instance.playerStates.Where(x => !Utils.PlayerById(x.TargetPlayerId).Is(RoleEnum.Pestilence) && x.VotedFor == ((Troll)role).TrolledPlayer.PlayerId).Select(x => x.TargetPlayerId).ToArray();
                    var pk = new PlayerMenu((x) => {
                        Utils.RpcMultiMurderPlayer(((Troll)role).Player, x);
                        role.PauseEndCrit = false;
                    }, (y) => {
                        return toKill.Contains(y.PlayerId);
                    });
                    Coroutines.Start(pk.Open(3f));
                }
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