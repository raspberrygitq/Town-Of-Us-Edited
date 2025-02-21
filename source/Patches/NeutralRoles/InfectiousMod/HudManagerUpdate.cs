using System.Linq;
using HarmonyLib;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.NeutralRoles.InfectiousMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Infectious)) return;
            var role = Role.GetRole<Infectious>(PlayerControl.LocalPlayer);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            __instance.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.InfectiousCD);

            if (role.InfectButton == null)
            {
                role.InfectButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.InfectButton.graphic.enabled = true;
                role.InfectButton.gameObject.SetActive(false);
            }

            role.InfectButton.graphic.sprite = TownOfUsEdited.InfectSprite;
            role.InfectButton.transform.localPosition = new Vector3(-1f, 1f, 0f);
            role.InfectButton.SetCoolDown(role.Cooldown, CustomGameOptions.InfectiousCD);

            role.InfectButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                var playerRole = Role.GetRole(player);
                if (role.Infected.Contains(player.PlayerId) && playerRole.InfectionState == 0) player.nameText().color = Color.blue;
                else if (playerRole.InfectionState == 1) player.nameText().color = Color.yellow;
                else if (playerRole.InfectionState == 2) player.nameText().color = Palette.Orange;
                else if (playerRole.InfectionState == 3) player.nameText().color = Color.red;
                else if (playerRole.InfectionState == 4) player.nameText().color = Color.black;
            }

            var notinfected = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !role.Infected.Contains(x.PlayerId))
                .ToList();

            if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
            else if (role.Player.IsLover()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover()).ToList());
            else Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
            
            if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayerInfect, role.InfectButton, float.NaN, notinfected);
            else if (role.Player.IsLover()) Utils.SetTarget(ref role.ClosestPlayerInfect, role.InfectButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !role.Infected.Contains(x.PlayerId)).ToList());
            else Utils.SetTarget(ref role.ClosestPlayerInfect, role.InfectButton, float.NaN, notinfected);
        }
    }
}
