using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.ImpostorRoles.SwooperMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Swooper)) return;
            var role = Role.GetRole<Swooper>(PlayerControl.LocalPlayer);
            if (role.SwoopButton == null)
            {
                role.SwoopButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.SwoopButton.graphic.enabled = true;
                role.SwoopButton.gameObject.SetActive(false);
                role.SwoopText = Object.Instantiate(__instance.KillButton.buttonLabelText, role.SwoopButton.transform);
                role.SwoopText.gameObject.SetActive(false);
                role.ButtonLabels.Add(role.SwoopText);
            }
            var phantom = RoleManager.Instance.GetRole(AmongUs.GameOptions.RoleTypes.Phantom).Cast<PhantomRole>();
            role.SwoopButton.graphic.sprite = phantom.Ability.Image;
            role.SwoopButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.SwoopButton.transform.localPosition = new Vector3(-2f, 1f, 0f);

            role.SwoopText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.SwoopText.text = "Swoop";
            role.SwoopText.SetOutlineColor(Palette.ImpostorRed);

            role.SwoopButton.graphic.SetCooldownNormalizedUvs();

            if (role.IsSwooped)
            {
                role.SwoopButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.SwoopDuration);
                return;
            }

            role.SwoopButton.SetCoolDown(role.SwoopTimer(), CustomGameOptions.SwoopCd);


            role.SwoopButton.graphic.color = Palette.EnabledColor;
            role.SwoopButton.graphic.material.SetFloat("_Desat", 0f);
            role.SwoopText.color = Palette.EnabledColor;
            role.SwoopText.material.SetFloat("_Desat", 0f);
        }
    }
}