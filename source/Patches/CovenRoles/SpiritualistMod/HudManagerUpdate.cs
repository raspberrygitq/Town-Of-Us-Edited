using System.Linq;
using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CovenRoles.SpiritualistMod
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
            }

            role.ControlButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            role.ControlButton.graphic.sprite = TownOfUs.SpiritualistControl;
            role.ControlButton.transform.localPosition = new Vector3(-2f, 1f, 0f);

            foreach (var player2 in PlayerControl.AllPlayerControls)
            {
                if (player2 == role.ControlledPlayer && !player2.Data.IsDead && !player2.Data.Disconnected) player2.nameText().color = Color.black;
            }

            // Set KillButton's cooldown
            role.ControlButton.SetCoolDown(role.KillCooldown, CustomGameOptions.CovenKCD);

            // Set the closest player for the Kill Button's targeting
            var notcoven = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !x.Is(Faction.Coven) && !x.Data.IsDead)
                .ToList();

            if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayer, role.ControlButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsControlled()).ToList());
            else if (PlayerControl.LocalPlayer.IsLover() && CustomGameOptions.ImpLoverKillTeammate) Utils.SetTarget(ref role.ClosestPlayer, role.ControlButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.IsControlled()).ToList());
            else if (PlayerControl.LocalPlayer.IsLover()) Utils.SetTarget(ref role.ClosestPlayer, role.ControlButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Coven) && !x.IsControlled()).ToList());
            else Utils.SetTarget(ref role.ClosestPlayer, role.ControlButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Coven) && !x.IsControlled()).ToList());
        }
    }
}