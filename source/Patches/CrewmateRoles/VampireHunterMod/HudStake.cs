﻿using System.Linq;
using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CrewmateRoles.VampireHunterMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudStake
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.VampireHunter)) return;
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var stakeButton = __instance.KillButton;

            var role = Role.GetRole<VampireHunter>(PlayerControl.LocalPlayer);

            if (role.UsesText == null && role.UsesLeft >= 0)
            {
                role.UsesText = Object.Instantiate(stakeButton.cooldownTimerText, stakeButton.transform);
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
            stakeButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            role.UsesText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            if (role.ButtonUsable) stakeButton.SetCoolDown(role.StakeTimer(), CustomGameOptions.StakeCd);
            else stakeButton.SetCoolDown(0f, CustomGameOptions.StakeCd);
            if (role.UsesLeft == 0) return;

            if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
            else if (role.Player.IsLover() && role.Player.Is(Faction.Madmates) && (!CustomGameOptions.MadmateKillEachOther || CustomGameOptions.GameMode == GameMode.Cultist)) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Impostors)).ToList());
            else if (role.Player.Is(Faction.Madmates) && (!CustomGameOptions.MadmateKillEachOther || CustomGameOptions.GameMode == GameMode.Cultist)) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Impostors)).ToList());
            else if (role.Player.IsLover()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover()).ToList());
            else Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);

            var renderer = stakeButton.graphic;
            if (role.ClosestPlayer != null && role.ButtonUsable)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                role.UsesText.color = Palette.EnabledColor;
                role.UsesText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
                role.UsesText.color = Palette.DisabledClear;
                role.UsesText.material.SetFloat("_Desat", 1f);
            }
        }
    }
}