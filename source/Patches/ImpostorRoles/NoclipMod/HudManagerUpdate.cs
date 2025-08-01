using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.ImpostorRoles.NoclipMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite Noclip => TownOfUsEdited.NoclipSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Noclip)) return;
            var role = Role.GetRole<Noclip>(PlayerControl.LocalPlayer);
            if (role.NoclipButton == null)
            {
                role.NoclipButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.NoclipButton.graphic.enabled = true;
                role.NoclipButton.gameObject.SetActive(false);
                role.NoclipText = Object.Instantiate(__instance.KillButton.buttonLabelText, role.NoclipButton.transform);
                role.NoclipText.gameObject.SetActive(false);
                role.ButtonLabels.Add(role.NoclipText);
            }
            role.NoclipButton.graphic.sprite = Noclip;
            role.NoclipButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.NoclipButton.transform.localPosition = new Vector3(-2f, 1f, 0f);

            role.NoclipText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.NoclipText.text = "Noclip";
            role.NoclipText.SetOutlineColor(Palette.ImpostorRed);

            role.NoclipButton.graphic.SetCooldownNormalizedUvs();
            if (role.Noclipped)
            {
                role.NoclipButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.NoclipDuration);
                return;
            }

            role.NoclipButton.SetCoolDown(role.NoclipTimer(), CustomGameOptions.NoclipCooldown);

            role.NoclipButton.graphic.color = Palette.EnabledColor;
            role.NoclipButton.graphic.material.SetFloat("_Desat", 0f);
            role.NoclipText.color = Palette.EnabledColor;
            role.NoclipText.material.SetFloat("_Desat", 0f);
        }
    }
}
