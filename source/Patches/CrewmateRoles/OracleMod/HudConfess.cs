﻿using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CrewmateRoles.OracleMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudConfess
    {
        public static Sprite Bless => TownOfUsEdited.BlessSprite;

        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Oracle)) return;
            var confessButton = __instance.KillButton;

            var role = Role.GetRole<Oracle>(PlayerControl.LocalPlayer);

            if (role.BlessButton == null)
            {
                role.BlessButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.BlessButton.graphic.enabled = true;
                role.BlessButton.gameObject.SetActive(false);
            }

            if (PlayerControl.LocalPlayer.Data.IsDead) role.BlessButton.SetTarget(null);

            var notBlessed = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => x != role.Blessed)
                .ToList();

            role.BlessButton.graphic.sprite = Bless;
            role.BlessButton.transform.localPosition = new Vector3(-2f, 0f, 0f);

            role.BlessButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            role.BlessButton.buttonLabelText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            role.BlessButton.SetCoolDown(role.BlessTimer(), CustomGameOptions.BlessCD);
            if (PlayerControl.LocalPlayer.moveable) Utils.SetTarget(ref role.ClosestBlessedPlayer, role.BlessButton, float.NaN, notBlessed);
            else role.BlessButton.SetTarget(null);

            role.BlessButton.graphic.SetCooldownNormalizedUvs();

            role.BlessButton.buttonLabelText.text = "Bless";
            role.BlessButton.buttonLabelText.SetOutlineColor(Patches.Colors.Oracle);

            confessButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            confessButton.buttonLabelText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            confessButton.buttonLabelText.text = "Confess";
            confessButton.buttonLabelText.SetOutlineColor(Patches.Colors.Oracle);

            confessButton.SetCoolDown(role.ConfessTimer(), CustomGameOptions.ConfessCd);

            var notConfessing = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => x != role.Confessor)
                .ToList();

            Utils.SetTarget(ref role.ClosestPlayer, confessButton, float.NaN, notConfessing);

            var renderer = role.BlessButton.graphic;
            var BlessText = role.BlessButton.buttonLabelText;
            if (role.ClosestBlessedPlayer != null)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                BlessText.color = Palette.EnabledColor;
                BlessText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
                BlessText.color = Palette.DisabledClear;
                BlessText.material.SetFloat("_Desat", 1f);
            }

            var renderer2 = confessButton.graphic;
            var ConfessText = confessButton.buttonLabelText;
            if (role.ClosestPlayer != null)
            {
                confessButton.graphic.color = Palette.EnabledColor;
                confessButton.graphic.material.SetFloat("_Desat", 0f);
                ConfessText.color = Palette.EnabledColor;
                ConfessText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                confessButton.graphic.color = Palette.DisabledClear;
                confessButton.graphic.material.SetFloat("_Desat", 1f);
                ConfessText.color = Palette.DisabledClear;
                ConfessText.material.SetFloat("_Desat", 1f);
            }
        }
    }
}