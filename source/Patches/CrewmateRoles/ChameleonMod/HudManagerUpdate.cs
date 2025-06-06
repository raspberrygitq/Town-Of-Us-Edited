using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.ChameleonMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            UpdateSwoopButton(__instance);
        }

        public static void UpdateSwoopButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Chameleon)) return;
            var swoopButton = __instance.KillButton;
            var swoopText = __instance.KillButton.buttonLabelText;

            var role = Role.GetRole<Chameleon>(PlayerControl.LocalPlayer);

            swoopButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            swoopText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            if (role.IsSwooped) swoopButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.ChamSwoopDuration);
            else swoopButton.SetCoolDown(role.SwoopTimer(), CustomGameOptions.ChamSwoopCooldown);

            var renderer = swoopButton.graphic;
            var label = swoopText;
            renderer.color = Palette.EnabledColor;
            renderer.material.SetFloat("_Desat", 0f);
            label.color = Palette.EnabledColor;
            label.material.SetFloat("_Desat", 0f);
        }
    }
}