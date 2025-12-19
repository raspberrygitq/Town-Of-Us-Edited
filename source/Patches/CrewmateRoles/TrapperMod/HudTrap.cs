using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CrewmateRoles.TrapperMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudTrap
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateTrapButton(__instance);
        }

        public static void UpdateTrapButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Trapper)) return;
            var trapButton = __instance.KillButton;

            var role = Role.GetRole<Trapper>(PlayerControl.LocalPlayer);
            var trapText = __instance.KillButton.buttonLabelText;

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(trapButton.cooldownTimerText, trapButton.transform);
                role.UsesText.gameObject.SetActive(false);
                role.UsesText.transform.localPosition = new Vector3(
                    role.UsesText.transform.localPosition.x + 0.26f,
                    role.UsesText.transform.localPosition.y + 0.29f,
                    role.UsesText.transform.localPosition.z);
                role.UsesText.transform.localScale = role.UsesText.transform.localScale * 0.65f;
                role.UsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.UsesText.fontStyle = TMPro.FontStyles.Bold;
            }
            if (role.UsesText != null)
            {
                role.UsesText.text = role.UsesLeft + "";
            }

            trapButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            role.UsesText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            trapText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            if (role.ButtonUsable) trapButton.SetCoolDown(role.TrapTimer(), CustomGameOptions.TrapCooldown);
            else trapButton.SetCoolDown(0f, CustomGameOptions.TrapCooldown);

            var renderer = trapButton.graphic;
            if (!trapButton.isCoolingDown && trapButton.gameObject.active && role.ButtonUsable)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                trapText.color = Palette.EnabledColor;
                trapText.material.SetFloat("_Desat", 0f);
                return;
            }

            renderer.color = Palette.DisabledClear;
            renderer.material.SetFloat("_Desat", 1f);
            trapText.color = Palette.DisabledClear;
            trapText.material.SetFloat("_Desat", 1f);
        }
    }
}
