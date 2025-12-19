using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CrewmateRoles.EngineerMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class KillButtonSprite
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Engineer)) return;
            if (PlayerControl.LocalPlayer.Is(Faction.Madmates)) return;

            var role = Role.GetRole<Engineer>(PlayerControl.LocalPlayer);
            var fixText = __instance.KillButton.buttonLabelText;

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(__instance.KillButton.cooldownTimerText, __instance.KillButton.transform);
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

            __instance.KillButton.SetCoolDown(0f, 10f);
            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            role.UsesText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            fixText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!ShipStatus.Instance) return;
            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            if (system == null) return;
            var sabActive = system.AnyActive;
            var renderer = __instance.KillButton.graphic;
            if (sabActive && role.ButtonUsable && __instance.KillButton.enabled && !PlayerControl.LocalPlayer.inVent)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                fixText.color = Palette.EnabledColor;
                fixText.material.SetFloat("_Desat", 0f);
                return;
            }

            renderer.color = Palette.DisabledClear;
            renderer.material.SetFloat("_Desat", 1f);
            fixText.color = Palette.DisabledClear;
            fixText.material.SetFloat("_Desat", 1f);
        }
    }
    [HarmonyPatch(typeof(HudManager))]
    public class MadmateFix
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Engineer)) return;
            if (!PlayerControl.LocalPlayer.Is(Faction.Madmates)) return;
            if (MeetingHud.Instance) return;
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.NetworkMode != NetworkModes.FreePlay) return;

            __instance.SabotageButton.Show();
            var position = __instance.KillButton.transform.localPosition;
            __instance.SabotageButton.transform.localPosition = new Vector3(0f, 1f, 0f);

            __instance.ImpostorVentButton.Show();
            var position2 = __instance.ImpostorVentButton.transform.localPosition;
            __instance.ImpostorVentButton.transform.localPosition = new Vector3(-2f, 0f, 0f);
        }
    }
}