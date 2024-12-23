using HarmonyLib;
using AmongUs.Data;
using InnerNet;
using System.Linq;
using TownOfUs.WerewolfRoles.TalkativeWolfMod.ChatPatch;

namespace TownOfUs.Patches
{
    // Also Patched the chat Cooldown because its damn annoying
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
    public class SendChat
    {
        public static bool bypassChat = false;
        public static bool Prefix(ChatController __instance)
        {
            if (CustomGameOptions.GameMode == GameMode.Chaos && AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started) bypassChat = true;
            else bypassChat = false;

            if (__instance.freeChatField.Text.ToLower().StartsWith("/ic ") && ((!PlayerControl.LocalPlayer.Is(Faction.Impostors) && (!PlayerControl.LocalPlayer.Is(Faction.Madmates) || !CustomGameOptions.MadmateCanChat)) || PlayerControl.LocalPlayer.Data.IsDead)) return false;
            else if (__instance.freeChatField.Text.ToLower().StartsWith("/lc ") && (!PlayerControl.LocalPlayer.IsLover() || PlayerControl.LocalPlayer.Data.IsDead)) return false;
            else if (__instance.freeChatField.Text.ToLower().StartsWith("/vc ") && (!PlayerControl.LocalPlayer.Is(RoleEnum.Vampire) || PlayerControl.LocalPlayer.Data.IsDead)) return false;
            else if (__instance.freeChatField.Text.ToLower().StartsWith("/cc ") && (!PlayerControl.LocalPlayer.Is(Faction.Coven) || PlayerControl.LocalPlayer.Data.IsDead)) return false;
            else if (__instance.freeChatField.Text.ToLower().StartsWith("/skc ") && (!PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller) || PlayerControl.LocalPlayer.Data.IsDead)) return false;
            else if (!__instance.freeChatField.Text.ToLower().StartsWith("/lc ") && !MeetingHud.Instance
            && !__instance.freeChatField.Text.ToLower().StartsWith("/ic ") && !LobbyBehaviour.Instance
            && !PlayerControl.LocalPlayer.Data.IsDead && !__instance.freeChatField.Text.ToLower().StartsWith("/vc ")
            && !__instance.freeChatField.Text.ToLower().StartsWith("/skc ") && !__instance.freeChatField.Text.ToLower().StartsWith("/cc") && !bypassChat) return false;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.TalkativeWolf) && ChatPatch.WordSaid != true && __instance.freeChatField.Text.ToLower().Contains(ChatPatch.Word) && MeetingHud.Instance
            && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var impos = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.Disconnected && x.Is(Faction.Impostors)).ToArray();
                foreach (var player in impos)
                {
                    var playerResults = "The Talkative Wolf has said his word! He can continue his day without problems...";
                    if (!string.IsNullOrWhiteSpace(playerResults) && player == PlayerControl.LocalPlayer)
                    {
                        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(player, playerResults);
                    }
                }
                ChatPatch.WordSaid = true;
                return true;
            }
            if (__instance.quickChatMenu.CanSend)
		    {
			    __instance.SendQuickChat();
		    }
		    else
		    {
			    if (__instance.quickChatMenu.IsOpen || string.IsNullOrWhiteSpace(__instance.freeChatField.Text) || DataManager.Settings.Multiplayer.ChatMode != QuickChatModes.FreeChatOrQuickChat)
			    {
				    return false;
			    }
			    __instance.SendFreeChat();
		    }
		    __instance.freeChatField.Clear();
		    __instance.quickChatMenu.Clear();
		    __instance.quickChatField.Clear();
		    __instance.UpdateChatMode();
            return false;
        }
    }
}