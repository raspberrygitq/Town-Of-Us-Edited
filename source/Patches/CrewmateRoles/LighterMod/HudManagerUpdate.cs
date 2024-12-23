using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.LighterMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudManagerUpdate
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateLightButton(__instance);
        }

        public static void UpdateLightButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Lighter)) return;
            var lightButton = __instance.KillButton;

            var role = Role.GetRole<Lighter>(PlayerControl.LocalPlayer);

            lightButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            if (role.UsingLight) lightButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.LightDuration);
            else lightButton.SetCoolDown(role.LightCooldown(), CustomGameOptions.LightCD);

            var renderer = lightButton.graphic;
            if (role.UsingLight || !lightButton.isCoolingDown)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }

            if (role.UsingLight)
            {
                role.StartLight();
            }
            else if (role.Enabled)
            {
                role.StopLight();
            }
        }
    }
}