using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowNormalMap))]
    public class MapColorOpenPatch
    {
        public static bool Prefix(MapBehaviour __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return true;
            if (PlayerControl.LocalPlayer == null) return true;
            if (PlayerControl.LocalPlayer.Data == null) return true;
            var role = Role.GetRole(PlayerControl.LocalPlayer);
            if (role == null) return true;
            if (__instance.IsOpen)
		    {
			    __instance.Close();
			    return false;
		    }
		    if (!PlayerControl.LocalPlayer.CanMove && !MeetingHud.Instance)
		    {
			    return false;
		    }
		    PlayerControl.LocalPlayer.SetPlayerMaterialColors(__instance.HerePoint);
		    if (PlayerControl.LocalPlayer.isTrackingPlayer)
		    {
			    if (PlayerControl.LocalPlayer.trackedPlayer.Data.Disconnected)
			    {
				    __instance.TrackedHerePoint.gameObject.SetActive(false);
			    }
			    else
			    {
				    __instance.SetTrackedHerePointColor(PlayerControl.LocalPlayer.trackedPlayerColorID);
				    __instance.TrackedHerePoint.gameObject.SetActive(true);
				    __instance.UpdateTrackedPosition();
			    }
		    }
		    __instance.GenericShow();
		    __instance.taskOverlay.Show();
            __instance.ColorControl.baseColor = role.Color;
		    __instance.ColorControl.SetColor(role.Color);
			DestroyableSingleton<HudManager>.Instance.SetHudActive(false);
            return false;
        }
    }

    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowSabotageMap))]
    public class MapColorOpenPatch2
    {
        public static bool Prefix(MapBehaviour __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return true;
            if (PlayerControl.LocalPlayer == null) return true;
            if (PlayerControl.LocalPlayer.Data == null) return true;
            var role = Role.GetRole(PlayerControl.LocalPlayer);
            if (role == null) return true;
            if (__instance.IsOpen)
		    {
			    __instance.Close();
			    return false;
		    }
		    if (!PlayerControl.LocalPlayer.CanMove && !MeetingHud.Instance)
		    {
			    return false;
		    }
		    if (__instance.specialInputHandler != null)
		    {
			    __instance.specialInputHandler.disableVirtualCursor = true;
		    }
		    PlayerControl.LocalPlayer.SetPlayerMaterialColors(__instance.HerePoint);
		    __instance.GenericShow();
		    __instance.infectedOverlay.gameObject.SetActive(true);
		    __instance.ColorControl.baseColor = role.Color;
		    __instance.ColorControl.SetColor(role.Color);
		    __instance.taskOverlay.Show();
		    DestroyableSingleton<HudManager>.Instance.SetHudActive(false);
		    ConsoleJoystick.SetMode_Sabotage();
            return false;
        }
    }
}