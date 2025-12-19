using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.NeutralRoles.VampireMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            var player = PlayerControl.LocalPlayer;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Vampire)) return;

            // Get the Vampire role instance
            var role = Role.GetRole<Vampire>(PlayerControl.LocalPlayer);

            // Check if the game state allows the KillButton to be active
            bool isKillButtonActive = __instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled;
            isKillButtonActive = isKillButtonActive && !MeetingHud.Instance && !player.Data.IsDead;
            isKillButtonActive = isKillButtonActive && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
            AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay);

            // Set KillButton's visibility
            __instance.KillButton.gameObject.SetActive(isKillButtonActive);

            if (role.BiteButton == null)
            {
                role.BiteButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.BiteButton.graphic.enabled = true;
                role.BiteButton.gameObject.SetActive(false);
            }

            var alivevamps = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => x.Is(RoleEnum.Vampire) && !x.Data.IsDead && !x.Data.Disconnected)
                .ToList();

            var vamps = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => x.Is(RoleEnum.Vampire))
                .ToList();

            role.BiteButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay)
                    && vamps.Count < CustomGameOptions.MaxVampiresPerGame
                    && alivevamps.Count == 1);
            role.BiteButton.buttonLabelText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay)
                    && vamps.Count < CustomGameOptions.MaxVampiresPerGame
                    && alivevamps.Count == 1);

            role.BiteButton.graphic.sprite = TownOfUsEdited.BiteSprite;
            role.BiteButton.transform.localPosition = new Vector3(-1f, 1f, 0f);

            role.BiteButton.buttonLabelText.text = "Bite";
            role.BiteButton.buttonLabelText.SetOutlineColor(Patches.Colors.Vampire);

            var notvamps = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !x.Is(RoleEnum.Vampire))
                .ToList();

            // Set KillButton's cooldown
            __instance.KillButton.SetCoolDown(role.BiteTimer(), CustomGameOptions.BiteCd);

            // Set the closest player for the Kill Button's targeting
            if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
            else if (PlayerControl.LocalPlayer.IsLover() && CustomGameOptions.ImpLoverKillTeammate) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover()).ToList());
            else if (PlayerControl.LocalPlayer.IsLover()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(RoleEnum.Vampire)).ToList());
            else Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(RoleEnum.Vampire)).ToList());

            Utils.SetTarget(ref role.ClosestPlayer, role.BiteButton, float.NaN, notvamps);
            
            // The Cooldown code is ran only once else, it will double the kill button speed up (because Time.deltaTime would be ran twice).
            role.BiteButton.SetCoolDown(role.Cooldown, CustomGameOptions.BiteCd);
            role.BiteButton.graphic.SetCooldownNormalizedUvs();

            if (role.ClosestPlayer != null)
            {
                role.BiteButton.graphic.color = Palette.EnabledColor;
                role.BiteButton.graphic.material.SetFloat("_Desat", 0f);
                role.BiteButton.buttonLabelText.color = Palette.EnabledColor;
                role.BiteButton.buttonLabelText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                role.BiteButton.graphic.color = Palette.DisabledClear;
                role.BiteButton.graphic.material.SetFloat("_Desat", 1f);
                role.BiteButton.buttonLabelText.color = Palette.DisabledClear;
                role.BiteButton.buttonLabelText.material.SetFloat("_Desat", 1f);
            }
        }
    }
}