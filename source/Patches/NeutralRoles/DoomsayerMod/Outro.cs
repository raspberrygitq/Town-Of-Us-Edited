using System.Linq;
using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.DoomsayerMod
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            if (!CustomGameOptions.NeutralEvilWinEndsGame) return;
            var role = Role.AllRoles.FirstOrDefault(x =>
                x.RoleType == RoleEnum.Doomsayer && ((Doomsayer) x).WonByGuessing);
            if (role == null) return;
            PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();
            foreach (var player in array) player.NameText().text = role.ColorString + player.NameText().text + "</color>";
            __instance.BackgroundBar.material.color = role.Color;
            var text = Object.Instantiate(__instance.WinText);
            text.text = "Doomsayer Wins!";
            text.color = role.Color;
            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
            if (role.Player.IsLover() && CustomGameOptions.NeutralEvilWinsLover) text.text += "<size=4>\n<color=#FF66CCFF>+ Lover</color></size>";
        }
    }
}