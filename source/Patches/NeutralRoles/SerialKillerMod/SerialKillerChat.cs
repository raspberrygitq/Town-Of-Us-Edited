using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;

namespace TownOfUsEdited.Patches.NeutralRoles.SerialKillerChat
{
    public class SerialKillerChat
    {
        public static bool skchat = false;
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class EnableChat
        {
            public static void Postfix(HudManager __instance)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller) && !__instance.Chat.isActiveAndEnabled)
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

                if ((MeetingHud.Instance || LobbyBehaviour.Instance) && !chatText.ToLower().StartsWith("/skc "))
                {
                    skchat = false;
                    return true;
                }
                
                if (chatText.ToLower().StartsWith("/skc "))
                {
                    skchat = true;
                    chatText = $"<color=#FFFFFF>{chatText[4..]}</color>";
                    Boolean shouldSeeMessage = (PlayerControl.LocalPlayer.Data.IsDead && Utils.ShowDeadBodies == true) || PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller) ||
                    sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId;
                    return shouldSeeMessage;
                }
                else skchat = false;
                return true;
            }
        }

        [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetText))]
        public class ChatColor
        {
            public static void Postfix(ChatBubble __instance)
            {
                if (LobbyBehaviour.Instance) return;
                if (skchat)
                {
                    __instance.transform.FindChild("Background").GetComponent<SpriteRenderer>().color = Patches.Colors.SerialKiller;
                    skchat = false;
                }
                return;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
        public class ChatMessage
        {
            public static void Postfix()
            {
                var sks = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.Disconnected && x.Is(RoleEnum.SerialKiller) && !x.Data.IsDead).ToList();
                foreach (var player in sks)
                {
                    var playerResults = "As a Serial Killer, use /skc to send messages to your teammates.\nUsage: /skc [message]";
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