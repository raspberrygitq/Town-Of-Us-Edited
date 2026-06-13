using HarmonyLib;
using System.Linq;
using TownOfUsEdited.NeutralRoles.ExecutionerMod;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.Patches.CrewmateRoles
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            if (Role.GetRoles(RoleEnum.Jester).Any(x => ((Jester)x).VotedOut) && CustomGameOptions.JesterWin == WinEndsGame.EndsGame) return;
            if (Role.GetRoles(RoleEnum.Troll).Any(x => ((Troll)x).TrolledVotedOut) && CustomGameOptions.TrollWin == WinEndsGame.EndsGame) return;
            if (Role.GetRoles(RoleEnum.Executioner).Any(x => ((Executioner)x).TargetVotedOut) && CustomGameOptions.ExecutionerWin == WinEndsGame.EndsGame) return;
            if (Role.GetRoles(RoleEnum.Doomsayer).Any(x => ((Doomsayer)x).WonByGuessing) && CustomGameOptions.DoomsayerWinEndsGame) return;
            if (Role.GetRoles(RoleEnum.Vulture).Any(x => ((Vulture)x).VultureWins) && CustomGameOptions.VultureWinEndsGame) return;

            if (!Role.CrewmateWins) return;
            var text = Object.Instantiate(__instance.WinText);
            text.text = "Crewmates Win!";
            text.color = Color.cyan;
            __instance.BackgroundBar.material.color = Color.cyan;
            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
        }
    }
}