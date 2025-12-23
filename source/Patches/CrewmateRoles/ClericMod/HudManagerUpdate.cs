using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CrewmateRoles.ClericMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Cleric)) return;
            var role = Role.GetRole<Cleric>(PlayerControl.LocalPlayer);

            if (role.CleanseButton == null)
            {
                role.CleanseButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.CleanseButton.graphic.enabled = true;
                role.CleanseButton.gameObject.SetActive(false);
                role.CleanseText = Object.Instantiate(__instance.KillButton.buttonLabelText, role.CleanseButton.transform);
                role.CleanseText.gameObject.SetActive(false);
                role.ButtonLabels.Add(role.CleanseText);
            }

            role.CleanseButton.graphic.sprite = TownOfUsEdited.CleanseSprite;
            role.CleanseButton.transform.localPosition = new Vector3(-2f, 0f, 0f);

            if (PlayerControl.LocalPlayer.Data.IsDead) role.CleanseButton.SetTarget(null);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            __instance.KillButton.buttonLabelText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            role.CleanseButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            role.CleanseText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            __instance.KillButton.buttonLabelText.text = "Barrier";
            __instance.KillButton.buttonLabelText.SetOutlineColor(Patches.Colors.Cleric);

            role.CleanseText.text = "Cleanse";
            role.CleanseText.SetOutlineColor(Patches.Colors.Cleric);

            __instance.KillButton.SetCoolDown(role.BarrierTimer(), CustomGameOptions.BarrierCD);
            role.CleanseButton.SetCoolDown(role.Cooldown, CustomGameOptions.BarrierCD);
            role.CleanseButton.graphic.SetCooldownNormalizedUvs();

            Utils.SetTarget(ref role.ClosestPlayer, role.CleanseButton, float.NaN);
            var cleanseRenderer = role.CleanseButton.graphic;
            var label = role.CleanseText;
            if (role.ClosestPlayer != null && role.CleanseButton.isActiveAndEnabled && PlayerControl.LocalPlayer.moveable)
            {
                cleanseRenderer.color = Palette.EnabledColor;
                cleanseRenderer.material.SetFloat("_Desat", 0f);
                label.color = Palette.EnabledColor;
                label.material.SetFloat("_Desat", 0f);
            }
            else
            {
                cleanseRenderer.color = Palette.DisabledClear;
                cleanseRenderer.material.SetFloat("_Desat", 1f);
                label.color = Palette.DisabledClear;
                label.material.SetFloat("_Desat", 1f);
            }

            if (role.Barriered == null) __instance.KillButton.SetTarget(role.ClosestPlayer);
            else
            {
                __instance.KillButton.SetTarget(null);
                var renderer = __instance.KillButton.graphic;
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                role.CleanseText.material.SetFloat("_Desat", 0f);
            }

            return;
        }
    }
}