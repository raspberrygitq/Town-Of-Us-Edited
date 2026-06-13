using HarmonyLib;
using TownOfUsEdited.CrewmateRoles.JailorMod;
using TownOfUsEdited.Modifiers.LoversMod;
using TownOfUsEdited.NeutralRoles.SerialKillerMod;
using TownOfUsEdited.NeutralRoles.VampireMod;
using TownOfUsEdited.Patches.CovenRoles;
using TownOfUsEdited.Patches.ImpostorRoles;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.HandleHud))]
    public class HandleHud
    {
        public static bool Prefix()
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.NetworkMode != NetworkModes.FreePlay) return true;
            bool chatToggled = HudManager.Instance.Chat.IsOpenOrOpening || (JailorChat.JailorChatButton != null && JailorChat.JailorChatButton.IsOpenOrOpening) ||
            (LoversChat.LoversChatButton != null && LoversChat.LoversChatButton.IsOpenOrOpening) || (SerialKillerChat.SerialKillerChatButton != null && SerialKillerChat.SerialKillerChatButton.IsOpenOrOpening) ||
            (VampireChat.VampireChatButton != null && VampireChat.VampireChatButton.IsOpenOrOpening) || (CovenChat.CovenChatButton != null && CovenChat.CovenChatButton.IsOpenOrOpening) ||
            (ImpostorChat.ImpostorChatButton != null && ImpostorChat.ImpostorChatButton.IsOpenOrOpening);
            if (chatToggled) return false;
            return true;
        }
    }
}