using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.CaptainMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudManagerUpdate
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateZoomButton(__instance);
        }

        public static void UpdateZoomButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Captain)) return;
            var ZoomButton = __instance.KillButton;

            var role = Role.GetRole<Captain>(PlayerControl.LocalPlayer);

            ZoomButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            if (role.Zooming) ZoomButton.SetCoolDown(role.TimeRemainingZoom, CustomGameOptions.ZoomDuration);
            else
            {
                ZoomButton.SetCoolDown(role.ZoomTimer(), CustomGameOptions.ZoomCooldown);
            }

            if (role.Zooming && MeetingHud.Instance)
            {
                ZoomButton.SetCoolDown(0f, CustomGameOptions.ZoomCooldown);
            }

            var renderer = ZoomButton.graphic;
            if (role.Zooming || !ZoomButton.isCoolingDown)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
        }
    }
}