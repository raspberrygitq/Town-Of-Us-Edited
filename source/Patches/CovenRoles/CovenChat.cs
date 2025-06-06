using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AmongUs.Data;
using AmongUs.QuickChat;
using Assets.CoreScripts;
using HarmonyLib;
using Reactor.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUsEdited.Patches.CovenRoles
{
    public class CovenChat
    {
        public static ChatController CovenChatButton;
        public static Transform Background;
        public static List<(PlayerControl, string, bool)> Messages = new List<(PlayerControl, string, bool)>();
        public static void UpdateCovenChat()
        {
            if (CovenChatButton && PlayerControl.LocalPlayer.Data.IsDead)
            {
                if (CovenChatButton.IsOpenOrOpening) ControllerManager.Instance.CloseOverlayMenu(CovenChatButton.name);
                Object.Destroy(CovenChatButton.gameObject);
                Object.Destroy(Background.gameObject);
            }
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!CustomGameOptions.CovenChat) return;
            if (!PlayerControl.LocalPlayer.Is(Faction.Coven))
            {
                if (CovenChatButton)
                {
                    if (CovenChatButton.IsOpenOrOpening) ControllerManager.Instance.CloseOverlayMenu(CovenChatButton.name);
                    Object.Destroy(CovenChatButton.gameObject);
                    Object.Destroy(Background.gameObject);
                }
                return;
            }
            if (CustomGameOptions.GameMode == GameMode.Chaos) return;
            if (AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay) return;
            List<PlayerControl> CovenTeam = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Coven) && !x.Data.IsDead && !x.Data.Disconnected).ToList();
            if (CovenTeam.Count < 2)
            {
                if (CovenChatButton)
                {
                    if (CovenChatButton.IsOpenOrOpening) ControllerManager.Instance.CloseOverlayMenu(CovenChatButton.name);
                    Object.Destroy(CovenChatButton.gameObject);
                    Object.Destroy(Background.gameObject);
                }
                return;
            }
            if (!CovenChatButton)
            {
                CovenChatButton = Object.Instantiate(HudManager.Instance.Chat, HudManager.Instance.Chat.transform.parent);
                CovenChatButton.name = "CovenChat";
                foreach (var bubble in CovenChatButton.chatBubblePool.activeChildren)
                {
                    Object.Destroy(bubble.gameObject);
                }
                CovenChatButton.chatBubblePool.activeChildren.Clear();
                CovenChatButton.chatButton.transform.Find("Inactive").GetComponent<SpriteRenderer>().color = Colors.Coven;
                CovenChatButton.chatButton.transform.Find("Active").GetComponent<SpriteRenderer>().color = Colors.Coven;
                CovenChatButton.chatButton.transform.Find("Selected").GetComponent<SpriteRenderer>().color = Colors.Coven;
                var container = CovenChatButton.chatScreen.transform.Find("ChatScreenContainer");
                container.transform.FindChild("Background").GetComponent<SpriteRenderer>().color = Colors.Coven;
            }
            if (!Background)
            {
                Background = Object.Instantiate(HudManager.Instance.SettingsButton.transform.GetChild(2), HudManager.Instance.SettingsButton.transform.GetChild(2).transform.parent);
                Background.name = "CovenChatBackground";
                var position2 = Background.transform.localPosition;
                Background.transform.localPosition = new Vector3(position2.x - 1.2f, position2.y, position2.z);
            }
            CovenChatButton.gameObject.SetActive(true);
            Background.gameObject.SetActive(true);
            var position = HudManager.Instance.Chat.transform.localPosition;
            if (!HudManager.Instance.Chat.isActiveAndEnabled)
            {
                CovenChatButton.transform.localPosition = position;
                Background.gameObject.SetActive(false);
                CovenChatButton.chatButton.transform.GetChild(3).gameObject.SetActive(true);
                return;
            }
            CovenChatButton.transform.localPosition = new Vector3(position.x - 0.85f, position.y, position.z);
            Background.gameObject.SetActive(true);
            CovenChatButton.chatButton.transform.GetChild(3).gameObject.SetActive(false);
        }

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.Toggle))]
        public class ToggleChat
        {
            public static bool Prefix(ChatController __instance)
            {
                if (CovenChatButton == null) return true;
                if (!CovenChatButton.isActiveAndEnabled) return true;
                if (CovenChatButton.IsOpenOrOpening && __instance != CovenChatButton) return false;
                if (__instance == CovenChatButton && !CovenChatButton.IsOpenOrOpening) //Open chat
                {
                    Coroutines.Start(WaitForSend(__instance));
                }
                return true;
            }

            public static IEnumerator WaitForSend(ChatController __instance)
            {
                yield return new WaitForSeconds(0.1f);
                while (Messages.Count > 0)
                {
                    var message = Messages[0];
                    ForceAddChat(__instance, message.Item1, message.Item2, message.Item3);
                    Messages.Remove(message);
                    yield return null;
                }
                yield break;
            }

            public static void ForceAddChat(ChatController __instance, PlayerControl srcPlayer, string chatText, bool censor)
            {
                NetworkedPlayerInfo data = PlayerControl.LocalPlayer.Data;
                NetworkedPlayerInfo data2 = srcPlayer.Data;
                if (data2 == null || data == null || (data2.IsDead && !data.IsDead))
                {
                    return;
                }
                ChatBubble pooledBubble = __instance.GetPooledBubble();
                try
                {
                    pooledBubble.transform.SetParent(__instance.scroller.Inner);
                    pooledBubble.transform.localScale = Vector3.one;
                    bool flag = srcPlayer == PlayerControl.LocalPlayer;
                    if (flag)
                    {
                        pooledBubble.SetRight();
                    }
                    else
                    {
                        pooledBubble.SetLeft();
                    }
                    bool didVote = MeetingHud.Instance && MeetingHud.Instance.DidVote(srcPlayer.PlayerId);
                    pooledBubble.SetCosmetics(data2);
                    __instance.SetChatBubbleName(pooledBubble, data2, data2.IsDead, didVote, PlayerNameColor.Get(data2), null);
                    if (censor && DataManager.Settings.Multiplayer.CensorChat)
                    {
                        chatText = BlockedWords.CensorWords(chatText, false);
                    }
                    pooledBubble.SetText(chatText);
                    pooledBubble.AlignChildren();
                    __instance.AlignAllBubbles();
                }
                catch (Exception message)
                {
                    ChatController.Logger.Error(message.ToString(), null);
                    __instance.chatBubblePool.Reclaim(pooledBubble);
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
		public static class SendChat
		{
			public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] ref string chatText)
			{
				if (CovenChatButton == null) return true;
                if (!CovenChatButton.isActiveAndEnabled) return true;
                if (!CovenChatButton.IsOpenOrOpening) return true;
                if (!__instance.AmOwner) return true;
                chatText = Regex.Replace(chatText, "<.*?>", string.Empty);
                if (string.IsNullOrWhiteSpace(chatText))
                {
                    return false;
                }
                if (DestroyableSingleton<HudManager>.Instance)
                {
                    CovenChatButton.AddChat(__instance, chatText, true);
                }
                if (chatText.IndexOf("who", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    DestroyableSingleton<UnityTelemetry>.Instance.SendWho();
                }
                Utils.Rpc(CustomRPC.SendCustomChat, __instance.PlayerId, chatText, "CovenChat");
                return false;
			}
		}

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
		public static class AddChat
		{
			public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl srcPlayer, [HarmonyArgument(1)] string chatText, [HarmonyArgument(2)] bool censor)
            {
                if (CovenChatButton == null) return true;
                if (!CovenChatButton.isActiveAndEnabled) return true;
                if (__instance != CovenChatButton) return true;
                if (CovenChatButton.IsOpenOrOpening) return true;
                Messages.Add((srcPlayer, chatText, censor)); // Avoids weird SetFlipXWithoutPet error, really sketchy fix, but couldn't find any better way :sob:
                var flag = srcPlayer == PlayerControl.LocalPlayer;
                if (__instance.notificationRoutine == null)
                {
                    __instance.notificationRoutine = __instance.StartCoroutine(__instance.BounceDot());
                }
                if (!flag)
                {
                    SoundManager.Instance.PlaySound(__instance.messageSound, false, 1f, null).pitch = 0.5f + (float)srcPlayer.PlayerId / 15f;
                    __instance.chatNotification.SetUp(srcPlayer, chatText);
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendQuickChat))]
		public static class SendQuickChat
		{
			public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] QuickChatPhraseBuilderResult data)
            {
                if (CovenChatButton == null) return true;
                if (!CovenChatButton.isActiveAndEnabled) return true;
                if (!CovenChatButton.IsOpenOrOpening) return true;
                if (!__instance.AmOwner) return true;
                string text = data.ToChatText();
                if (string.IsNullOrWhiteSpace(text) || data == null || !data.IsValid())
                {
                    return false;
                }
                if (DestroyableSingleton<HudManager>.Instance)
                {
                    CovenChatButton.AddChat(__instance, text, false);
                }
                if (data.ToChatText().IndexOf("who", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    DestroyableSingleton<UnityTelemetry>.Instance.SendWho();
                }
                Utils.Rpc(CustomRPC.SendCustomChat, __instance.PlayerId, text, "CovenChat");
                return false;
            }
		}

        [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetText))]
        public class ChatColor
        {
            public static void Postfix(ChatBubble __instance)
            {
                if (LobbyBehaviour.Instance) return;
                if ((CovenChatButton != null && CovenChatButton.isActiveAndEnabled && CovenChatButton.chatBubblePool.activeChildren.Contains(__instance)) || (__instance.TextArea.text.Contains("[Coven Chat]") && PlayerControl.LocalPlayer.Data.IsDead))
                {
                    __instance.Background.color = Colors.Coven;
                    __instance.NameText.color = Colors.Coven;
                }
            }
        }
    }
}