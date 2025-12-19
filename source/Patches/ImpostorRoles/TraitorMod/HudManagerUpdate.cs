using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.ImpostorRoles.TraitorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Traitor)) return;
            var role = Role.GetRole<Traitor>(PlayerControl.LocalPlayer);
            if (role.ChangeRoleButton == null)
            {
                role.ChangeRoleButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ChangeRoleButton.graphic.enabled = true;
                role.ChangeRoleButton.gameObject.SetActive(false);
                role.ChangeText = Object.Instantiate(__instance.KillButton.buttonLabelText, role.ChangeRoleButton.transform);
                role.ChangeText.gameObject.SetActive(false);
                role.ButtonLabels.Add(role.ChangeText);
            }
            role.ChangeRoleButton.graphic.sprite = TownOfUsEdited.SwitchRole;
            role.ChangeRoleButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.ChangeRoleButton.transform.localPosition = new Vector3(-2f, 1f, 0f);

            role.ChangeText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.ChangeText.text = "Select";
            role.ChangeText.SetOutlineColor(Palette.ImpostorRed);
            role.ChangeRoleButton.SetCoolDown(0f, 1f);

            role.ChangeRoleButton.graphic.color = Palette.EnabledColor;
            role.ChangeRoleButton.graphic.material.SetFloat("_Desat", 0f);
            role.ChangeText.color = Palette.EnabledColor;
            role.ChangeText.material.SetFloat("_Desat", 0f);
        }
    }
}