using HarmonyLib;
using TMPro;
using TownOfUsEdited.Roles.Modifiers;
using UnityEngine;

namespace TownOfUsEdited.Modifiers.DisperserMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update
    {
        public static void Postfix(HudManager __instance)
        {
            UpdateButtonButton(__instance);
        }

        private static void UpdateButtonButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Disperser)) return;

            var role = Modifier.GetModifier<Disperser>(PlayerControl.LocalPlayer);

            if (role.DisperseButton == null)
            {
                role.DisperseButton = Object.Instantiate(__instance.KillButton, __instance.transform.parent);
                role.DisperseButton.GetComponentsInChildren<TextMeshPro>()[0].text = "";
                role.DisperseButton.graphic.enabled = true;
                role.DisperseButton.graphic.sprite = TownOfUsEdited.DisperseSprite;
            }

            role.DisperseButton.graphic.sprite = TownOfUsEdited.DisperseSprite;

            role.DisperseButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            role.DisperseButton.buttonLabelText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            role.DisperseButton.SetCoolDown(role.StartTimer(), 10f);
            role.DisperseButton.graphic.SetCooldownNormalizedUvs();

            role.DisperseButton.buttonLabelText.text = "Disperse";
            role.DisperseButton.buttonLabelText.SetOutlineColor(Palette.ImpostorRed);

            var renderer = role.DisperseButton.graphic;


            if (PlayerControl.LocalPlayer.Is(ModifierEnum.ButtonBarry) || PlayerControl.LocalPlayer.Is(ModifierEnum.Satellite))
            {
                var position = __instance.KillButton.transform.position;
                role.DisperseButton.transform.position = new Vector3(
                    Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f, position.y,
                    position.z);
            }
            else
            {
                if (__instance.UseButton != null)
                {
                    var position = __instance.UseButton.transform.position;
                    role.DisperseButton.transform.position = new Vector3(
                        Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f, position.y,
                        position.z);
                }
                else
                {
                    var position = __instance.PetButton.transform.position;
                    role.DisperseButton.transform.position = new Vector3(
                        Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f, position.y,
                        position.z);
                }
            }

            if (!role.ButtonUsed)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                role.DisperseButton.buttonLabelText.color = Palette.EnabledColor;
                role.DisperseButton.buttonLabelText.material.SetFloat("_Desat", 0f);
                return;
            }

            renderer.color = Palette.DisabledClear;
            renderer.material.SetFloat("_Desat", 1f);
            role.DisperseButton.buttonLabelText.color = Palette.EnabledColor;
            role.DisperseButton.buttonLabelText.material.SetFloat("_Desat", 0f);
        }
    }
}