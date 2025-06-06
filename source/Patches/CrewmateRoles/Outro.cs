using System.Linq;
using HarmonyLib;
using TownOfUsEdited.Roles;
using TownOfUsEdited.Extensions;
using UnityEngine;

namespace TownOfUsEdited.Patches.CrewmateRoles
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public class Outro
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
            }
            if (!Role.CrewmateWins) return;
            var text = Object.Instantiate(__instance.WinText);
            if (CustomGameOptions.GameMode != GameMode.Werewolf)
            {
                text.text = "Crewmates Win!";
                text.color = Color.cyan;
                __instance.BackgroundBar.material.color = Color.cyan;
            }
            else
            {
                text.text = "Villagers Win!";
                text.color = Patches.Colors.Villager;
                __instance.BackgroundBar.material.color = Patches.Colors.Villager;
            }
            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
        }
    }
}