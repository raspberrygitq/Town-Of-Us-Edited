using System.Linq;
using HarmonyLib;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.Patches.ImpostorRoles
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
                if (Role.GetRoles(RoleEnum.SoulCollector).Any(x => ((SoulCollector)x).CollectedSouls)) return;
            }
            if (!Role.ImpostorWins) return;
            var text = Object.Instantiate(__instance.WinText);
            PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();
            foreach (var player in array)
            {
                if (CustomGameOptions.GameMode == GameMode.Werewolf)
                {
                    player.NameText().text = "<color=#A86629FF>" + player.NameText().text + "</color>";
                    player.SetBodyType(PlayerBodyTypes.Seeker);
                }
                else if (CustomGameOptions.GameMode == GameMode.Chaos)
                {
                    player.SetBodyType(PlayerBodyTypes.Seeker);
                }
            }
            if (CustomGameOptions.GameMode != GameMode.Werewolf)
            {
                text.text = "Impostors Win!";
                text.color = Palette.ImpostorRed;
                __instance.BackgroundBar.material.color = Palette.ImpostorRed;
            }
            else
            {
                text.text = "Werewolves Win!";
                text.color = Patches.Colors.Werewolf;
                __instance.BackgroundBar.material.color = Patches.Colors.Werewolf;
            }
            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
        }
    }
}