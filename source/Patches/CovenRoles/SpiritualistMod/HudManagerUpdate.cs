using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Patches;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CovenRoles.SpiritualistMod
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
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Spiritualist)) return;

            var role = Role.GetRole<Spiritualist>(PlayerControl.LocalPlayer);

            if (role.ControlButton == null)
            {
                role.ControlButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ControlButton.graphic.enabled = true;
                role.ControlButton.gameObject.SetActive(false);
                role.ControlText = Object.Instantiate(__instance.KillButton.buttonLabelText, role.ControlButton.transform);
                role.ControlText.gameObject.SetActive(false);
                role.ButtonLabels.Add(role.ControlText);
            }

            role.ControlButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.ControlText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.ControlButton.graphic.sprite = TownOfUsEdited.SpiritualistControl;
            role.ControlButton.transform.localPosition = new Vector3(-2f, 1f, 0f);

            role.ControlText.text = "Control";
            role.ControlText.SetOutlineColor(Colors.Coven);

            foreach (var player2 in PlayerControl.AllPlayerControls)
            {
                if (player2 == role.ControlledPlayer && !player2.Data.IsDead && !player2.Data.Disconnected) player2.nameText().color = Color.black;
            }

            // Set KillButton's cooldown
            role.ControlButton.SetCoolDown(role.KillCooldown, CustomGameOptions.CovenKCD);
            role.ControlButton.graphic.SetCooldownNormalizedUvs();

            // Set the closest player for the Kill Button's targeting
            var notcoven = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !x.Is(Faction.Coven) && !x.Data.IsDead)
                .ToList();

            if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayer, role.ControlButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsControlled()).ToList());
            else if (PlayerControl.LocalPlayer.IsLover() && CustomGameOptions.ImpLoverKillTeammate) Utils.SetTarget(ref role.ClosestPlayer, role.ControlButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.IsControlled()).ToList());
            else if (PlayerControl.LocalPlayer.IsLover()) Utils.SetTarget(ref role.ClosestPlayer, role.ControlButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Coven) && !x.IsControlled()).ToList());
            else Utils.SetTarget(ref role.ClosestPlayer, role.ControlButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Coven) && !x.IsControlled()).ToList());

            var labelrender = role.ControlText;
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
    }
}