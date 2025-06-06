using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.ImpostorRoles.MorphlingMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite SampleSprite => TownOfUsEdited.SampleSprite;


        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Morphling)) return;
            var role = Role.GetRole<Morphling>(PlayerControl.LocalPlayer);
            var shapeshifter = RoleManager.Instance.GetRole(AmongUs.GameOptions.RoleTypes.Shapeshifter).Cast<ShapeshifterRole>();
            Sprite MorphSprite = shapeshifter.Ability.Image;
            if (role.MorphButton == null)
            {
                role.MorphButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.MorphButton.graphic.enabled = true;
                role.MorphButton.graphic.sprite = SampleSprite;
                role.MorphButton.gameObject.SetActive(false);
                role.MorphText = Object.Instantiate(__instance.KillButton.buttonLabelText, role.MorphButton.transform);
                role.MorphText.gameObject.SetActive(false);
                role.ButtonLabels.Add(role.MorphText);
            }

            if (role.MorphButton.graphic.sprite != SampleSprite && role.MorphButton.graphic.sprite != MorphSprite)
            {
                role.MorphButton.graphic.sprite = SampleSprite;
                role.MorphText.text = "Sample";
            }

            role.MorphButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.MorphText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.MorphText.SetOutlineColor(Palette.ImpostorRed);

            role.MorphButton.transform.localPosition = new Vector3(-2f, 1f, 0f);
            if (role.MorphButton.graphic.sprite == SampleSprite)
            {
                role.MorphText.text = "Sample";
                role.MorphButton.SetCoolDown(0f, 1f);
                Utils.SetTarget(ref role.ClosestPlayer, role.MorphButton);
                var labelrender = role.MorphText;
                if (role.ClosestPlayer != null)
                {
                    labelrender.color = Palette.EnabledColor;
                    labelrender.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    labelrender.color = Palette.DisabledClear;
                    labelrender.material.SetFloat("_Desat", 1f);
                }
            }
            else
            {
                if (role.Morphed)
                {
                    role.MorphButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.MorphlingDuration);
                    return;
                }

                role.MorphButton.SetCoolDown(role.MorphTimer(), CustomGameOptions.MorphlingCd);
                role.MorphButton.graphic.color = Palette.EnabledColor;
                role.MorphButton.graphic.material.SetFloat("_Desat", 0f);
                role.MorphText.color = Palette.EnabledColor;
                role.MorphText.material.SetFloat("_Desat", 0f);
                role.MorphText.text = "Morph";
            }
        }
    }
}
