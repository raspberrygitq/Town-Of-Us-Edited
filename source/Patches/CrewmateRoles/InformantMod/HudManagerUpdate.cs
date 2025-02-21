using HarmonyLib;
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

            if (role.VitalsButton == null)
            {
                role.VitalsButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.VitalsButton.graphic.enabled = true;
                role.VitalsButton.gameObject.SetActive(false);
            }

            role.VitalsButton.graphic.sprite = TownOfUsEdited.VitalsSprite;
            role.VitalsButton.transform.localPosition = new Vector3(-2f, 0f, 0f);

            role.VitalsButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            __instance.KillButton.graphic.sprite = TownOfUsEdited.AdminSprite;

            var renderer = __instance.KillButton.graphic;
            var renderer2 = role.VitalsButton.graphic;
            renderer.color = Palette.EnabledColor;
            renderer.material.SetFloat("_Desat", 0f);
            renderer2.color = Palette.EnabledColor;
            renderer2.material.SetFloat("_Desat", 0f);
        }
    }
}