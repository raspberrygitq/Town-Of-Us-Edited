using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.ImpostorRoles.VenererMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Venerer)) return;
            var role = Role.GetRole<Venerer>(PlayerControl.LocalPlayer);
            if (role.AbilityButton == null)
            {
                role.AbilityButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.AbilityButton.graphic.enabled = true;
                role.AbilityButton.gameObject.SetActive(false);

                role.AbilityText = Object.Instantiate(__instance.KillButton.buttonLabelText, role.AbilityButton.transform);
                role.AbilityText.gameObject.SetActive(false);
                role.ButtonLabels.Add(role.AbilityText);

                var labelCamouflage = Object.Instantiate(__instance.KillButton.buttonLabelText, role.AbilityButton.transform);
                labelCamouflage.gameObject.SetActive(false);
                role.ButtonLabels.Add(labelCamouflage);

                var labelSprint = Object.Instantiate(__instance.KillButton.buttonLabelText, role.AbilityButton.transform);
                labelSprint.gameObject.SetActive(false);
                role.ButtonLabels.Add(labelSprint);

                var labelFreeze = Object.Instantiate(__instance.KillButton.buttonLabelText, role.AbilityButton.transform);
                labelFreeze.gameObject.SetActive(false);
                role.ButtonLabels.Add(labelFreeze);
            }
            if (role.Kills == 0) role.AbilityButton.graphic.sprite = TownOfUsEdited.NoAbilitySprite;
            else if (role.Kills == 1) role.AbilityButton.graphic.sprite = TownOfUsEdited.CamouflageSprite;
            else if (role.Kills == 2) role.AbilityButton.graphic.sprite = TownOfUsEdited.CamoSprintSprite;
            else role.AbilityButton.graphic.sprite = TownOfUsEdited.CamoSprintFreezeSprite;
            role.AbilityButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            var shouldShow = (__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                                         && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                                         && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                                             AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay);
            var index = Mathf.Clamp(role.Kills, 0, 3);
            for (var i = 0; i < role.ButtonLabels.Count; i++)
            {
                var b = role.ButtonLabels[i];
                b.gameObject.SetActive(shouldShow && i == index);
            }
            var selectedLabel = role.ButtonLabels[Mathf.Clamp(index, 0, role.ButtonLabels.Count - 1)];
            role.AbilityText = selectedLabel;

            switch (index)
            {
                case 0:
                    role.AbilityText.text = "None";
                    break;
                case 1:
                    role.AbilityText.text = "Camouflage";
                    break;
                case 2:
                    role.AbilityText.text = "Sprint";
                    break;
                default:
                    role.AbilityText.text = "Freeze";
                    break;
            }

            role.AbilityText.SetOutlineColor(Palette.ImpostorRed);

            role.AbilityButton.transform.localPosition = new Vector3(-2f, 1f, 0f);

            role.AbilityButton.graphic.SetCooldownNormalizedUvs();
            if (role.IsCamouflaged)
            {
                role.AbilityButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.AbilityDuration);
                return;
            }

            var cooldownTimer = role.AbilityTimer();
            if (role.Kills > 0 && PlayerControl.LocalPlayer.moveable && cooldownTimer == 0f)
            {
                role.AbilityButton.SetCoolDown(cooldownTimer, CustomGameOptions.AbilityCd);
                role.AbilityButton.graphic.color = Palette.EnabledColor;
                role.AbilityButton.graphic.material.SetFloat("_Desat", 0f);
                role.AbilityText.color = Palette.EnabledColor;
                role.AbilityText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                role.AbilityButton.SetCoolDown(cooldownTimer, CustomGameOptions.AbilityCd);
                role.AbilityButton.graphic.color = Palette.DisabledClear;
                role.AbilityButton.graphic.material.SetFloat("_Desat", 1f);
                role.AbilityText.color = Palette.DisabledClear;
                role.AbilityText.material.SetFloat("_Desat", 1f);
            }
        }
    }
}