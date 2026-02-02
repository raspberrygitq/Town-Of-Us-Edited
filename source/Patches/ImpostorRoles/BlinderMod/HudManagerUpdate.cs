using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.ImpostorRoles.BlinderMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            var player = PlayerControl.LocalPlayer;
            // Check if there is only one player or if local player is null or dead
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Blinder)) return;

            // Get the Blinder role instance
            var role = Role.GetRole<Blinder>(PlayerControl.LocalPlayer);

            if (role.BlindButton == null)
            {
                role.BlindButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.BlindButton.graphic.enabled = true;
                role.BlindButton.gameObject.SetActive(false);
            }

            role.BlindButton.graphic.sprite = TownOfUsEdited.Blind;
            role.BlindButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.BlindButton.buttonLabelText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.BlindButton.buttonLabelText.text = "Blind";
            role.BlindButton.buttonLabelText.SetOutlineColor(Palette.ImpostorRed);

            var position = __instance.KillButton.transform.localPosition;
            role.BlindButton.transform.localPosition = new Vector3(position.x,
                position.y, position.z);

            // Set KillButton's cooldown
            var notimps = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Data.IsImpostor() && !x.Is(Faction.Madmates)).ToList();
            if (role.Blinding) role.BlindButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.BlindDuration);
            else role.BlindButton.SetCoolDown(role.BlindTimer(), CustomGameOptions.BlindCD);
            Utils.SetTarget(ref role.ClosestPlayer, role.BlindButton, float.NaN, notimps);
            role.BlindButton.graphic.SetCooldownNormalizedUvs();

            var labelrender = role.BlindButton.buttonLabelText;
            if (role.ClosestPlayer != null || role.Blinding)
            {
                role.BlindButton.graphic.color = Palette.EnabledColor;
                role.BlindButton.graphic.material.SetFloat("_Desat", 0f);
                labelrender.color = Palette.EnabledColor;
                labelrender.material.SetFloat("_Desat", 0f);
            }
            else
            {
                role.BlindButton.graphic.color = Palette.DisabledClear;
                role.BlindButton.graphic.material.SetFloat("_Desat", 1f);
                labelrender.color = Palette.DisabledClear;
                labelrender.material.SetFloat("_Desat", 1f);
            }
        }
    }
}