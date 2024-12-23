using TownOfUs.Patches;
using HarmonyLib;
using TownOfUs.Extensions;

namespace ChatStartPatch
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ResetChatSent
    {
        public static void Prefix()
        {
            if (LobbyBehaviour.Instance && Start.impsent != false)
            {
                Start.impsent = false;
            }
            if (LobbyBehaviour.Instance && Start.vampsent != false)
            {
                Start.vampsent = false;
            }
            if (LobbyBehaviour.Instance && Start.sksent != false)
            {
                Start.sksent = false;
            }
            if (LobbyBehaviour.Instance && Start.covensent != false)
            {
                Start.covensent = false;
            }
            if (LobbyBehaviour.Instance && Start.madsent != false)
            {
                Start.madsent = false;
            }
            if (LobbyBehaviour.Instance && Start.lovsent != false)
            {
                Start.lovsent = false;
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