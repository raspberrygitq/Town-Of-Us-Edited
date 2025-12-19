using HarmonyLib;
using TownOfUsEdited.Patches;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CrewmateRoles.InformantMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudManagerUpdate
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateKillButtons(__instance);
        }

        public static void UpdateKillButtons(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Informant)) return;

            var role = Role.GetRole<Informant>(PlayerControl.LocalPlayer);
            var AdminText = __instance.KillButton.buttonLabelText;

            if (role.VitalsButton == null)
            {
                role.VitalsButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.VitalsButton.graphic.enabled = true;
                role.VitalsButton.gameObject.SetActive(false);
                role.VitalsText = Object.Instantiate(__instance.KillButton.buttonLabelText, role.VitalsButton.transform);
                role.VitalsText.gameObject.SetActive(false);
                role.ButtonLabels.Add(role.VitalsText);
            }

            role.VitalsButton.graphic.sprite = HudManager.Instance.UseButton.fastUseSettings[ImageNames.VitalsButton].Image;
            role.VitalsButton.transform.localPosition = new Vector3(-2f, 0f, 0f);

            role.VitalsButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.VitalsText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            __instance.KillButton.graphic.sprite = HudManager.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton].Image;

            AdminText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.VitalsText.text = "Vitals";
            role.VitalsText.SetOutlineColor(Colors.Informant);

            var renderer = __instance.KillButton.graphic;
            var renderer2 = role.VitalsButton.graphic;
            if (!CamouflageUnCamouflage.CommsEnabled)
            {
                AdminText.color = Palette.EnabledColor;
                AdminText.material.SetFloat("_Desat", 0f);
                role.VitalsText.color = Palette.EnabledColor;
                role.VitalsText.material.SetFloat("_Desat", 0f);
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                renderer2.color = Palette.EnabledColor;
                renderer2.material.SetFloat("_Desat", 0f);
            }
            else
            {
                AdminText.color = Palette.DisabledClear;
                AdminText.material.SetFloat("_Desat", 1f);
                role.VitalsText.color = Palette.DisabledClear;
                role.VitalsText.material.SetFloat("_Desat", 1f);
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
                renderer2.color = Palette.DisabledClear;
                renderer2.material.SetFloat("_Desat", 1f);
            }
        }
    }
}