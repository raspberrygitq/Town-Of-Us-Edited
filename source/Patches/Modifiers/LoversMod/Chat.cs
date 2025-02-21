using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace TownOfUsEdited.Modifiers.LoversMod
{
    public static class Chat
    {
        public static bool loverchat = false;
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
        public static class AddChat
        {
            public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer, ref string chatText)
            {
                if (__instance != HudManager.Instance.Chat) return true;
                var localPlayer = PlayerControl.LocalPlayer;
                if (localPlayer == null) return true;
                if ((MeetingHud.Instance || LobbyBehaviour.Instance) && !chatText.ToLower().StartsWith("/lc "))
                {
                    loverchat = false;
                    return true;
                }
                if (chatText.ToLower().StartsWith("/lc "))
                {
                    loverchat = true;
                    chatText = chatText[4..];
                    Boolean shouldSeeMessage = (localPlayer.Data.IsDead && Utils.ShowDeadBodies == true) || localPlayer.IsLover() ||
                    sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId;
                    return shouldSeeMessage;
                }
                else loverchat = false;
                return true;
            }
        }

        [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetText))]
        public class ChatColor
        {
            public static void Postfix(ChatBubble __instance)
            {
                if (LobbyBehaviour.Instance) return;
                if (loverchat)
                {
                    __instance.transform.FindChild("Background").GetComponent<SpriteRenderer>().color = Patches.Colors.Lovers;
                    loverchat = false;
                }
                return;
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class EnableChat
        {
            public static void Postfix(HudManager __instance)
            {
                if (PlayerControl.LocalPlayer.IsLover() & !__instance.Chat.isActiveAndEnabled)
                    __instance.Chat.SetVisible(true);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
        public class ChatMessage
        {
            public static void Postfix()
            {
                var lovers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.Disconnected && x.IsLover() && !x.Data.IsDead).ToList();
                foreach (var player in lovers)
                {
                    var playerResults = "As a Lover, use /lc to send messages to your second part.\nUsage: /lc [message]";
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