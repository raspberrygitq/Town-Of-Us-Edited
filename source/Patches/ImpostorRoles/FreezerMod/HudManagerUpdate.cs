using System.Linq;
using HarmonyLib;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.ImpostorRoles.FreezerMod
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
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Freezer)) return;

            // Get the Freezer role instance
            var role = Role.GetRole<Freezer>(PlayerControl.LocalPlayer);

            if (role.FreezeButton == null)
            {
                role.FreezeButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.FreezeButton.graphic.enabled = true;
                role.FreezeButton.gameObject.SetActive(false);
                role.FreezeText = Object.Instantiate(__instance.KillButton.buttonLabelText, role.FreezeButton.transform);
                role.FreezeText.gameObject.SetActive(false);
                role.ButtonLabels.Add(role.FreezeText);
            }

            role.FreezeButton.graphic.sprite = TownOfUsEdited.Freeze;
            role.FreezeButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.FreezeText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.FreezeText.text = "Freeze";
            role.FreezeText.SetOutlineColor(Palette.ImpostorRed);

            var position = __instance.KillButton.transform.localPosition;
            role.FreezeButton.transform.localPosition = new Vector3(position.x,
                position.y, position.z);

            // Set KillButton's cooldown
            var notimps = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Data.IsImpostor() && !x.Is(Faction.Madmates)).ToList();
            if (role.Freezing) role.FreezeButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.BlindDuration);
            else role.FreezeButton.SetCoolDown(role.FreezeTimer(), CustomGameOptions.BlindCD);
            Utils.SetTarget(ref role.ClosestPlayer, role.FreezeButton, float.NaN, notimps);
            role.FreezeButton.graphic.SetCooldownNormalizedUvs();

            var labelrender = role.FreezeText;
            if (role.ClosestPlayer != null || role.Freezing)
            {
                role.FreezeButton.graphic.color = Palette.EnabledColor;
                role.FreezeButton.graphic.material.SetFloat("_Desat", 0f);
                labelrender.color = Palette.EnabledColor;
                labelrender.material.SetFloat("_Desat", 0f);
            }
            else
            {
                role.FreezeButton.graphic.color = Palette.DisabledClear;
                role.FreezeButton.graphic.material.SetFloat("_Desat", 1f);
                labelrender.color = Palette.DisabledClear;
                labelrender.material.SetFloat("_Desat", 1f);
            }
        }
    }
}