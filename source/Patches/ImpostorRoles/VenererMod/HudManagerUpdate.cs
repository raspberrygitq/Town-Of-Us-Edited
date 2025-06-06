using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.ImpostorRoles.VenererMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite NoneSprite => TownOfUsEdited.NoAbilitySprite;
        public static Sprite CamoSprite => TownOfUsEdited.CamouflageSprite;
        public static Sprite CamoSprintSprite => TownOfUsEdited.CamoSprintSprite;
        public static Sprite CamoSprintFreezeSprite => TownOfUsEdited.CamoSprintFreezeSprite;

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
            }
            if (role.Kills == 0) role.AbilityButton.graphic.sprite = NoneSprite;
            else if (role.Kills == 1) role.AbilityButton.graphic.sprite = CamoSprite;
            else if (role.Kills == 2) role.AbilityButton.graphic.sprite = CamoSprintSprite;
            else role.AbilityButton.graphic.sprite = CamoSprintFreezeSprite;
            role.AbilityButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.AbilityText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.AbilityText.text = "Ability";
            role.AbilityText.SetOutlineColor(Palette.ImpostorRed);

            role.AbilityButton.transform.localPosition = new Vector3(-2f, 1f, 0f);

            if (role.IsCamouflaged)
            {
                role.AbilityButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.AbilityDuration);
                return;
            }

            var labelrender = role.AbilityText;
            if (role.Kills > 0)
            {
                role.AbilityButton.SetCoolDown(role.AbilityTimer(), CustomGameOptions.AbilityCd);
                role.AbilityButton.graphic.color = Palette.EnabledColor;
                role.AbilityButton.graphic.material.SetFloat("_Desat", 0f);
                labelrender.color = Palette.EnabledColor;
                labelrender.material.SetFloat("_Desat", 0f);
            }
            else
            {
                role.AbilityButton.SetCoolDown(0, CustomGameOptions.AbilityCd);
                role.AbilityButton.graphic.color = Palette.DisabledClear;
                role.AbilityButton.graphic.material.SetFloat("_Desat", 1f);
                labelrender.color = Palette.DisabledClear;
                labelrender.material.SetFloat("_Desat", 1f);
            }

        }
    }
}