using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.ImpostorRoles.BomberMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Bomber)) return;
            var role = Role.GetRole<Bomber>(PlayerControl.LocalPlayer);
            if (role.PlantButton == null)
            {
                role.PlantButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.PlantButton.graphic.enabled = true;
                role.PlantButton.graphic.sprite = TownOfUsEdited.PlantSprite;
                role.PlantButton.gameObject.SetActive(false);
            }

            role.PlantButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            role.PlantButton.buttonLabelText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.PlantButton.transform.localPosition = new Vector3(-2f, 1f, 0f);

            if (role.Detonating)
            {
                role.PlantButton.graphic.sprite = TownOfUsEdited.DetonateSprite;
                role.PlantButton.buttonLabelText.text = "Detonating";
                role.DetonateTimer();
                role.PlantButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.DetonateDelay);
                role.PlantButton.buttonLabelText.color = Palette.EnabledColor;
                role.PlantButton.buttonLabelText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                role.PlantButton.graphic.sprite = TownOfUsEdited.PlantSprite;
                role.PlantButton.buttonLabelText.text = "Place";
                if (!role.Detonated) role.DetonateKillStart();
                if (role.KillCooldown > 0)
                {
                    role.PlantButton.graphic.color = Palette.DisabledClear;
                    role.PlantButton.graphic.material.SetFloat("_Desat", 1f);
                    role.PlantButton.buttonLabelText.color = Palette.DisabledClear;
                    role.PlantButton.buttonLabelText.material.SetFloat("_Desat", 1f);
                }
                else
                {
                    role.PlantButton.graphic.color = Palette.EnabledColor;
                    role.PlantButton.graphic.material.SetFloat("_Desat", 0f);
                    role.PlantButton.buttonLabelText.color = Palette.EnabledColor;
                    role.PlantButton.buttonLabelText.material.SetFloat("_Desat", 0f);
                }
                role.PlantButton.SetCoolDown(role.KillCooldown, GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
            }

            if (role.PlantButton.graphic.sprite == TownOfUsEdited.PlantSprite) role.PlantButton.SetCoolDown(role.KillCooldown,
                GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
            else role.PlantButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.DetonateDelay);
            
            role.PlantButton.graphic.SetCooldownNormalizedUvs();
        }
    }
}
