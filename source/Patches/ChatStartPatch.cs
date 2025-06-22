using HarmonyLib;
using TownOfUsEdited.Extensions;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ResetChatSent
    {
        public static bool welcomesent = false;
        public static void Prefix()
        {
            // Welcome message
            if (PlayerControl.LocalPlayer != null && LobbyBehaviour.Instance && !welcomesent)
            {
                var message = $"Welcome to <color=#EE9D01>Town Of Us</color><b><color=#AA00FF> Edited</color></b> {PlayerControl.LocalPlayer.Data.PlayerName}!\n\nTo view the commands list, type /help.\nTo know more about the mod, visit the github.\nTo see roles / modifiers descriptions, type /r [role/modifier name].";
                if (!string.IsNullOrWhiteSpace(message))
                {
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message, false);
                    welcomesent = true;
                }
            }

            if (LobbyBehaviour.Instance && PlayerControl.LocalPlayer.IsDev() && DevFeatures.isRandom
            && !PlayerControl.LocalPlayer.GetDefaultOutfit().PlayerName.IsNullOrWhiteSpace()
            && PlayerControl.LocalPlayer.GetDefaultOutfit().PlayerName != DevFeatures.SavedOutfit.PlayerName)
            {
                PlayerControl.LocalPlayer.SetName(DevFeatures.SavedOutfit.PlayerName);
			    PlayerControl.LocalPlayer.SetColor(DevFeatures.SavedOutfit.ColorId);
                PlayerControl.LocalPlayer.CmdCheckName(DevFeatures.SavedOutfit.PlayerName);
			    PlayerControl.LocalPlayer.CmdCheckColor((byte)DevFeatures.SavedOutfit.ColorId);
			    PlayerControl.LocalPlayer.RpcSetPet(DevFeatures.SavedOutfit.PetId);
			    PlayerControl.LocalPlayer.RpcSetHat(DevFeatures.SavedOutfit.HatId);
			    PlayerControl.LocalPlayer.RpcSetSkin(DevFeatures.SavedOutfit.SkinId);
			    PlayerControl.LocalPlayer.RpcSetVisor(DevFeatures.SavedOutfit.VisorId);
            }
        }
    }
}