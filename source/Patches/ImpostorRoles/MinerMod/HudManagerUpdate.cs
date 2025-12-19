using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.ImpostorRoles.MinerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Miner)) return;
            var role = Role.GetRole<Miner>(PlayerControl.LocalPlayer);
            if (role.MineButton == null)
            {
                role.MineButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.MineButton.graphic.enabled = true;
                role.MineButton.gameObject.SetActive(false);
            }

            role.MineButton.graphic.sprite = TownOfUsEdited.MineSprite;
            role.MineButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            role.MineButton.buttonLabelText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.MineButton.transform.localPosition = new Vector3(-2f, 1f, 0f);

            role.MineButton.SetCoolDown(role.MineTimer(), CustomGameOptions.MineCd);
            role.MineButton.graphic.SetCooldownNormalizedUvs();

            role.MineButton.buttonLabelText.text = "Mine";

            var hits = Physics2D.OverlapBoxAll(PlayerControl.LocalPlayer.transform.position, role.VentSize, 0);
            hits = hits.ToArray().Where(c =>
                    (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5)
                .ToArray();
            if (hits.Count == 0 && PlayerControl.LocalPlayer.moveable == true && !role.coolingDown)
            {
                role.MineButton.graphic.color = Palette.EnabledColor;
                role.MineButton.graphic.material.SetFloat("_Desat", 0f);
                role.MineButton.buttonLabelText.color = Palette.EnabledColor;
                role.MineButton.buttonLabelText.material.SetFloat("_Desat", 0f);
                role.CanPlace = true;
            }
            else
            {
                role.MineButton.graphic.color = Palette.DisabledClear;
                role.MineButton.graphic.material.SetFloat("_Desat", 1f);
                role.MineButton.buttonLabelText.color = Palette.DisabledClear;
                role.MineButton.buttonLabelText.material.SetFloat("_Desat", 1f);
                role.CanPlace = false;
            }
        }
    }
}