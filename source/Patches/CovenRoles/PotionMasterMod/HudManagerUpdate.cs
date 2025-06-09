using HarmonyLib;
using TownOfUsEdited.Patches;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CovenRoles.PotionMasterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            var player = PlayerControl.LocalPlayer;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.PotionMaster)) return;

            var role = Role.GetRole<PotionMaster>(PlayerControl.LocalPlayer);

            if (role.PotionButton == null)
            {
                role.PotionButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.PotionButton.graphic.enabled = true;
                role.PotionButton.gameObject.SetActive(false);
                role.PotionText = Object.Instantiate(__instance.KillButton.buttonLabelText, role.PotionButton.transform);
                role.PotionText.gameObject.SetActive(false);
                role.ButtonLabels.Add(role.PotionText);
            }

            role.PotionText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.PotionText.SetOutlineColor(Colors.Coven);

            if (role.Potion == "null")
            {
                role.PotionType = "None";
                role.RegenTask();
            }
            else
            {
                role.PotionType = role.Potion;
                role.RegenTask();
            }

            role.PotionButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            if (role.Potion == "null")
            {
                role.PotionButton.graphic.sprite = TownOfUsEdited.Potion;
                role.PotionText.text = "Potion";
            }
            else
            {
                role.PotionButton.graphic.sprite = TownOfUsEdited.Drink;
                role.PotionText.text = "Drink";
            }
            role.PotionButton.transform.localPosition = new Vector3(-2f, 1f, 0f);

            // Set KillButton's cooldown
            if (role.UsingPotion)
            {
                role.PotionButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.PotionDuration);
            }
            else role.PotionButton.SetCoolDown(role.PotionTimer(), CustomGameOptions.PotionCD);

            role.PotionButton.graphic.SetCooldownNormalizedUvs();

            role.PotionButton.graphic.color = Palette.EnabledColor;
            role.PotionButton.graphic.material.SetFloat("_Desat", 0f);
            var labelrender = role.PotionText;
            labelrender.color = Palette.EnabledColor;
            labelrender.material.SetFloat("_Desat", 0f);
        }
    }
}