using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public class ForceGameEndOutro
    {
        public static void Postfix(EndGameManager __instance)
        {
            if (CustomGameOptions.NeutralEvilWinEndsGame)
            {
                if (Role.GetRoles(RoleEnum.Jester).Any(x => ((Jester)x).VotedOut)) return;
                if (Role.GetRoles(RoleEnum.Troll).Any(x => ((Troll)x).TrolledVotedOut)) return;
                if (Role.GetRoles(RoleEnum.Executioner).Any(x => ((Executioner)x).TargetVotedOut)) return;
                if (Role.GetRoles(RoleEnum.Doomsayer).Any(x => ((Doomsayer)x).WonByGuessing)) return;
                if (Role.GetRoles(RoleEnum.Vulture).Any(x => ((Vulture)x).VultureWins)) return;
                if (Role.GetRoles(RoleEnum.SoulCollector).Any(x => ((SoulCollector)x).CollectedSouls)) return;
            }
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
                System.Console.WriteLine("GAME OVER REASON: Host Forced End Game");
                Utils.EndGame();
            }
        }
    }
}