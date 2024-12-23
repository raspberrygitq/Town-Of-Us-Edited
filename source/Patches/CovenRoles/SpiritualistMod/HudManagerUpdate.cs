using System.Linq;
using HarmonyLib;
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
            var killButton = __instance.KillButton;

            if (role.SabotageButton == null)
            {
                role.SabotageButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.SabotageButton.graphic.enabled = true;
                role.SabotageButton.gameObject.SetActive(false);
            }

            if (role.ControlButton == null)
            {
                role.ControlButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ControlButton.graphic.enabled = true;
                role.ControlButton.gameObject.SetActive(false);
            }

            killButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            role.SabotageButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            role.ControlButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
                    
            role.SabotageButton.graphic.sprite = TownOfUs.SabotageCoven;
            role.ControlButton.graphic.sprite = TownOfUs.SpiritualistControl;
            role.ControlButton.transform.localPosition = new Vector3(-2f, 1f, 0f);
            if (!PlayerControl.LocalPlayer.Data.IsDead)
            {
                role.SabotageButton.transform.localPosition = new Vector3(-1f, 1f, 0f);
            }
            else
            {
                var position = __instance.KillButton.transform.localPosition;
                role.SabotageButton.transform.localPosition = new Vector3(position.x,
                position.y, position.z);
            }

            // Set KillButton's cooldown
            killButton.SetCoolDown(role.KillTimer(), CustomGameOptions.CovenKCD);
            role.ControlButton.SetCoolDown(role.Cooldown, CustomGameOptions.CovenKCD);
            role.SabotageButton.SetCoolDown(0f, 1f);

            // Set the closest player for the Kill Button's targeting
            var notcoven = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !x.Is(Faction.Coven) && !x.Data.IsDead)
                .ToList();

            if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
            else if (PlayerControl.LocalPlayer.IsLover() && CustomGameOptions.ImpLoverKillTeammate) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover()).ToList());
            else if (PlayerControl.LocalPlayer.IsLover()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Coven)).ToList());
            else Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Coven)).ToList());

            if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayer, role.ControlButton);
            else if (PlayerControl.LocalPlayer.IsLover() && CustomGameOptions.ImpLoverKillTeammate) Utils.SetTarget(ref role.ClosestPlayer, role.ControlButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover()).ToList());
            else if (PlayerControl.LocalPlayer.IsLover()) Utils.SetTarget(ref role.ClosestPlayer, role.ControlButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Coven)).ToList());
            else Utils.SetTarget(ref role.ClosestPlayer, role.ControlButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Coven)).ToList());
            
            role.SabotageButton.graphic.color = Palette.EnabledColor;
            role.SabotageButton.graphic.material.SetFloat("_Desat", 0f);
        }
    }
}