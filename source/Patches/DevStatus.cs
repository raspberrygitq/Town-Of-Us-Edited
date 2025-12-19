using AmongUs.Data;
using HarmonyLib;
using Reactor.Utilities;
using System.Collections;
using System.Linq;
using TownOfUsEdited.Extensions;
using UnityEngine;

namespace TownOfUsEdited.Patches
{
    public class DevStatus
    {
        public static bool enabled = true;
        public static bool hidden = false;
        public static Color ArtistColor = new Color32(231, 76, 60, 255);
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class Title
        {
            public static void Postfix()
            {
                if (!LobbyBehaviour.Instance || !enabled) return;
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    var friendCode = player.FriendCode;
                    bool isLocal = player == PlayerControl.LocalPlayer;
                    if (isLocal) friendCode = EOSManager.Instance.FriendCode;
                    if (isLocal && hidden) friendCode = "[HIDDEN]" + friendCode;
                    var data = DevFeatures.Data;
                    bool hasStatus = friendCode == data.MainDev || data.Dev.Any(x => x == friendCode) ||
                    data.Artist.Any(x => x == friendCode) || data.Tester.Any(x => x == friendCode) ||
                    data.Vip.Any(x => x == friendCode);
                    if (friendCode == data.MainDev)
                    {
                        if (!player.nameText().text.Contains("Dev")) player.nameText().text = player.nameText().text + "<size=2>\n(Main Dev)</size>";
                        if (player.nameText().color != Palette.Orange) player.nameText().color = Palette.Orange;
                    }
                    else if (data.Dev.Any(x => x == friendCode))
                    {
                        if (!player.nameText().text.Contains("Dev")) player.nameText().text = player.nameText().text + "<size=2>\n(Dev)</size>";
                        if (player.nameText().color != Palette.ImpostorRed) player.nameText().color = Palette.ImpostorRed;
                    }
                    else if (data.Artist.Any(x => x == friendCode))
                    {
                        if (!player.nameText().text.Contains("Artist")) player.nameText().text = player.nameText().text + "<size=2>\n(Artist)</size>";
                        if (player.nameText().color != ArtistColor) player.nameText().color = ArtistColor;
                    }
                    else if (data.Tester.Any(x => x == friendCode))
                    {
                        if (!player.nameText().text.Contains("Tester")) player.nameText().text = player.nameText().text + "<size=2>\n(Tester)</size>";
                        if (player.nameText().color != Color.grey) player.nameText().color = Color.grey;
                    }
                    else if (data.Vip.Any(x => x == friendCode))
                    {
                        if (!player.nameText().text.Contains("VIP")) player.nameText().text = player.nameText().text + "<size=2>\n(VIP)</size>";
                        if (player.nameText().color != Color.yellow) player.nameText().color = Color.yellow;
                    }
                    else if (!hasStatus)
                    {
                        player.nameText().text = player.Data.PlayerName;
                        player.nameText().color = Palette.White;
                    }

                    if (hasStatus)
                    {
                        if (DataManager.Settings.Accessibility.ColorBlindMode) player.nameText().transform.localPosition = new Vector3(0f, 0.15f, -0.5f);
                        else player.nameText().transform.localPosition = new Vector3(0f, 0f, 0f);
                    }
                    else player.nameText().transform.localPosition = new Vector3(0f, 0f, 0f);
                }
            }
        }

        public static IEnumerator CheckForStatus()
        {
            while (PlayerControl.LocalPlayer == null) yield return null;
            Utils.Rpc(CustomRPC.CheckForStatus, PlayerControl.LocalPlayer.PlayerId);
            yield break;
        }

        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
        public class OnPlayerJoined
        {
            public static void Postfix()
            {
                if (enabled)
                {
                    Coroutines.Start(CheckForStatus());
                }
            }
        }

        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
        public class OnGameJoined
        {
            public static void Postfix()
            {
                if (enabled)
                {
                    Coroutines.Start(CheckForStatus());
                }
            }
        }
    }
}