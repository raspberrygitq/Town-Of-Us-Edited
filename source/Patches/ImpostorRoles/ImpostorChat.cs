using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;

namespace TownOfUs.Patches.ImpostorRoles.ImpostorChat
{
    public class ImpostorChat
    {
        public static bool impchat = false;
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class EnableChat
        {
            public static void Postfix(HudManager __instance)
            {
                if ((PlayerControl.LocalPlayer.Is(Faction.Impostors) || (PlayerControl.LocalPlayer.Is(Faction.Madmates) && CustomGameOptions.MadmateCanChat))
                && !__instance.Chat.isActiveAndEnabled)
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

                if ((MeetingHud.Instance || LobbyBehaviour.Instance) && !chatText.ToLower().StartsWith("/ic "))
                {
                    impchat = false;
                    return true;
                }
                
                if (chatText.ToLower().StartsWith("/ic "))
                {
                    impchat = true;
                    chatText = chatText[4..];
                    Boolean shouldSeeMessage = (PlayerControl.LocalPlayer.Data.IsDead && Utils.ShowDeadBodies == true) || PlayerControl.LocalPlayer.Is(Faction.Impostors) ||
                    (PlayerControl.LocalPlayer.Is(Faction.Madmates) && CustomGameOptions.MadmateCanChat) || sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId;
                    return shouldSeeMessage;
                }
                else impchat = false;
                return true;
            }
        }

        [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetText))]
        public class ChatColor
        {
            public static void Postfix(ChatBubble __instance)
            {
                if (LobbyBehaviour.Instance) return;
                if (impchat)
                {
                    __instance.transform.FindChild("Background").GetComponent<SpriteRenderer>().color = Palette.ImpostorRed;
                    impchat = false;
                }
                return;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
        public class ChatMessage
        {
            public static void Postfix()
            {
                var impos = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.Disconnected && x.Is(Faction.Impostors) && !x.Data.IsDead).ToList();
                foreach (var player in impos)
                {
                    var playerResults = "As an Impostor, use /ic to send messages to your teammates.\nUsage: /ic [message]";
                    if (!string.IsNullOrWhiteSpace(playerResults) && player == PlayerControl.LocalPlayer)
                    {
                        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(player, playerResults);
                        return;
                    }
                }
                var madmates = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.Disconnected && x.Is(Faction.Madmates) && !x.Data.IsDead).ToList();
                if (CustomGameOptions.MadmateCanChat)
                {
                    foreach (var player in madmates)
                    {
                        var playerResults = "You are in the Impostors team, use /ic to send messages to your teammates.\nUsage: /ic [message]";
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
}