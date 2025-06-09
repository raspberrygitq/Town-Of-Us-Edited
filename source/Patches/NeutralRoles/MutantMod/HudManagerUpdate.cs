using System.Linq;
using HarmonyLib;
using TownOfUsEdited.Patches;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.NeutralRoles.MutantMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudManagerUpdate
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateKillButton(__instance);
            UpdateTransformButton(__instance);
        }
        public static void UpdateKillButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mutant)) return;

            var player = PlayerControl.LocalPlayer;
            var role = Role.GetRole<Mutant>(PlayerControl.LocalPlayer);

            // Check if the local player is the Mutant
            if (!player.Is(RoleEnum.Mutant))
            {
                return;
            }

            // Check if the game state allows the KillButton to be active
            bool isKillButtonActive = __instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled;
            isKillButtonActive = isKillButtonActive && !MeetingHud.Instance && !player.Data.IsDead;
            isKillButtonActive = isKillButtonActive && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
            AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay);

            // Set KillButton's visibility
            __instance.KillButton.gameObject.SetActive(isKillButtonActive);
            if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
            else if (role.Player.IsLover()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover()).ToList());
            else Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);

            if (role.IsTransformed == true)
            {
                __instance.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.MutantKCD);
            }
            else __instance.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.TransformKCD);
        }
        public static void UpdateTransformButton (HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mutant)) return;

            var role = Role.GetRole<Mutant>(PlayerControl.LocalPlayer);
            if (role.TransformButton == null)
            {
                role.TransformButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.TransformButton.graphic.enabled = true;
                role.TransformButton.graphic.sprite = TownOfUsEdited.TransformSprite;
                role.TransformButton.gameObject.SetActive(false);
                role.TransformText = Object.Instantiate(__instance.KillButton.buttonLabelText, role.TransformButton.transform);
                role.TransformText.gameObject.SetActive(false);
                role.ButtonLabels.Add(role.TransformText);
            }
            if (role.TransformButton.graphic.sprite != TownOfUsEdited.TransformSprite &&
                role.TransformButton.graphic.sprite != TownOfUsEdited.UnTransformSprite)
                role.TransformButton.graphic.sprite = TownOfUsEdited.TransformSprite;

            if (role.TransformButton.graphic.sprite == TownOfUsEdited.UnTransformSprite && role.IsTransformed == false)
                role.TransformButton.graphic.sprite = TownOfUsEdited.TransformSprite;

            if (role.IsTransformed == true)
            {
                role.TransformButton.graphic.sprite = TownOfUsEdited.UnTransformSprite;
            }

            role.TransformButton.transform.localPosition = new Vector3(-1f, 1f, 0f);
            role.TransformButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.TransformText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.TransformText.text = "Transform";
            role.TransformText.SetOutlineColor(Colors.Mutant);

            if (role.IsTransformed != true)
            {
                role.TransformButton.SetCoolDown(role.TransformTimer(), CustomGameOptions.TransformCD);
            }

            role.TransformButton.graphic.SetCooldownNormalizedUvs();

            var renderer = role.TransformButton.graphic;
            var label = role.TransformText;
            renderer.color = Palette.EnabledColor;
            renderer.material.SetFloat("_Desat", 0f);
            label.color = Palette.EnabledColor;
            label.material.SetFloat("_Desat", 0f);

            if (role.IsTransformed == true && MeetingHud.Instance)
            {
                PlayerControl.LocalPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                Utils.Rpc(CustomRPC.UnTransform, PlayerControl.LocalPlayer.PlayerId);
                role.IsTransformed = false;
                __instance.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.MutantKCD);
            }
        }
    }
}