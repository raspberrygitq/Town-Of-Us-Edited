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

            var role = Role.GetRole<Chameleon>(PlayerControl.LocalPlayer);

            swoopButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            swoopButton.buttonLabelText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            swoopButton.buttonLabelText.text = "Swoop";
            swoopButton.buttonLabelText.SetOutlineColor(Patches.Colors.Chameleon);

            if (role.IsSwooped) swoopButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.ChamSwoopDuration);
            else swoopButton.SetCoolDown(role.SwoopTimer(), CustomGameOptions.ChamSwoopCooldown);

            var renderer = swoopButton.graphic;
            var label = swoopButton.buttonLabelText;
            if (!role.IsSwooped && !role.coolingDown)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                label.color = Palette.EnabledColor;
                label.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
                label.color = Palette.DisabledClear;
                label.material.SetFloat("_Desat", 1f);
            }
        }
    }
}