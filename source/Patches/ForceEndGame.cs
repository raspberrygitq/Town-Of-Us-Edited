using HarmonyLib;
using Reactor.Utilities;
using System.Linq;
using TownOfUsEdited.NeutralRoles.ExecutionerMod;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public class ForceGameEndOutro
    {
        public static void Postfix(EndGameManager __instance)
        {
            if (Role.GetRoles(RoleEnum.Jester).Any(x => ((Jester)x).VotedOut) && CustomGameOptions.JesterWin == WinEndsGame.EndsGame) return;
            if (Role.GetRoles(RoleEnum.Troll).Any(x => ((Troll)x).TrolledVotedOut) && CustomGameOptions.TrollWin == WinEndsGame.EndsGame) return;
            if (Role.GetRoles(RoleEnum.Executioner).Any(x => ((Executioner)x).TargetVotedOut) && CustomGameOptions.ExecutionerWin == WinEndsGame.EndsGame) return;
            if (Role.GetRoles(RoleEnum.Doomsayer).Any(x => ((Doomsayer)x).WonByGuessing) && CustomGameOptions.DoomsayerWinEndsGame) return;
            if (Role.GetRoles(RoleEnum.Vulture).Any(x => ((Vulture)x).VultureWins) && CustomGameOptions.VultureWinEndsGame) return;

            if (!Role.ForceGameEnd) return;
            var text = Object.Instantiate(__instance.WinText);
            text.text = "The Host Ended The Game";
            text.color = Color.white;
            __instance.BackgroundBar.material.color = Color.white;
            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ForceGameEnd
    {
        public static void Postfix()
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            if (!AmongUsClient.Instance.AmHost) return;
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.G) && Input.GetKey(KeyCode.Return))
            {
                Role.ForceGameEnd = true;
                Utils.Rpc(CustomRPC.ForceEndGame);
                PluginSingleton<TownOfUsEdited>.Instance.Log.LogMessage("GAME OVER REASON: Host Forced End Game");
                Utils.EndGame();
            }
        }
    }
}