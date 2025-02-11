using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;

namespace TownOfUs.Patches.CovenRoles.CovenChat
{
    public class CovenChat
    {
        public static bool covenchat = false;
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class EnableChat
        {
            public static void Postfix(HudManager __instance)
            {
                if (PlayerControl.LocalPlayer.Is(Faction.Coven) && !__instance.Chat.isActiveAndEnabled)
                    __instance.Chat.SetVisible(true);
            }
        }

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
        public class AddChat
        {
            public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer, ref string chatText)
            {
                if (__instance != HudManager.Instance.Chat)
                    return true;

                if ((MeetingHud.Instance || LobbyBehaviour.Instance) && !chatText.ToLower().StartsWith("/cc "))
                {
                    covenchat = false;
                    return true;
                }
                
                if (chatText.ToLower().StartsWith("/cc "))
                {
                    covenchat = true;
                    chatText = chatText[4..];
                    Boolean shouldSeeMessage = (PlayerControl.LocalPlayer.Data.IsDead && Utils.ShowDeadBodies == true) || PlayerControl.LocalPlayer.Is(Faction.Coven) ||
                    sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId;
                    return shouldSeeMessage;
                }
                else covenchat = false;
                return true;
            }
        }

        [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetText))]
        public class ChatColor
        {
            public static void Postfix(ChatBubble __instance)
            {
                if (LobbyBehaviour.Instance) return;
                if (covenchat)
                {
                    __instance.transform.FindChild("Background").GetComponent<SpriteRenderer>().color = Patches.Colors.Coven;
                    covenchat = false;
                }
                return;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
        public class ChatMessage
        {
            public static void Postfix()
            {
                var coven = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.Disconnected && x.Is(Faction.Coven) && !x.Data.IsDead).ToList();
                foreach (var player in coven)
                {
                    var playerResults = "As a Coven member, use /cc to send messages to your teammates.\nUsage: /cc [message]";
                    if (!string.IsNullOrWhiteSpace(playerResults) && player == PlayerControl.LocalPlayer)
                    {
                        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(player, playerResults);
                        return;
                    }
                }
            }
        }
    }
}