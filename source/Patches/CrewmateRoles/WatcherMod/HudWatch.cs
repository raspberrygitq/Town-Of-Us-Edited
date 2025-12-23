using System.Linq;
using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CrewmateRoles.WatcherMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudTrack
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Watcher)) return;
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var watchButton = __instance.KillButton;

            var role = Role.GetRole<Watcher>(PlayerControl.LocalPlayer);

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(watchButton.cooldownTimerText, watchButton.transform);
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
            watchButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            watchButton.buttonLabelText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            role.UsesText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            watchButton.buttonLabelText.text = "Watch";
            watchButton.buttonLabelText.SetOutlineColor(Patches.Colors.Lookout);

            if (role.ButtonUsable) watchButton.SetCoolDown(role.WatchTimer(), CustomGameOptions.WatcherCooldown);
            else watchButton.SetCoolDown(0f, CustomGameOptions.WatcherCooldown);
            if (role.UsesLeft == 0) return;

            var notWatching = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !role.Watching.ContainsKey(x.PlayerId))
                .ToList();

            Utils.SetTarget(ref role.ClosestPlayer, watchButton, float.NaN, notWatching);

            var renderer = watchButton.graphic;
            if (role.ClosestPlayer != null && role.ButtonUsable && PlayerControl.LocalPlayer.moveable)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                role.UsesText.color = Palette.EnabledColor;
                role.UsesText.material.SetFloat("_Desat", 0f);
                watchButton.buttonLabelText.color = Palette.EnabledColor;
                watchButton.buttonLabelText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
                role.UsesText.color = Palette.DisabledClear;
                role.UsesText.material.SetFloat("_Desat", 1f);
                watchButton.buttonLabelText.color = Palette.DisabledClear;
                watchButton.buttonLabelText.material.SetFloat("_Desat", 1f);
            }
        }
    }
}