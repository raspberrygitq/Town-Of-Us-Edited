using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CovenRoles.HexMasterMod
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
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.HexMaster)) return;

            var role = Role.GetRole<HexMaster>(PlayerControl.LocalPlayer);
            var HexButton = __instance.KillButton;

            if (role.SabotageButton == null)
            {
                role.SabotageButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.SabotageButton.graphic.enabled = true;
                role.SabotageButton.gameObject.SetActive(false);
            }

            if (role.HexBombButton == null)
            {
                role.HexBombButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.HexBombButton.graphic.enabled = true;
                role.HexBombButton.gameObject.SetActive(false);
            }

            HexButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            role.SabotageButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            role.HexBombButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
                    
            role.SabotageButton.graphic.sprite = TownOfUs.SabotageCoven;
            role.HexBombButton.graphic.sprite = TownOfUs.HexBomb;
            role.HexBombButton.transform.localPosition = new Vector3(-2f, 1f, 0f);
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

            foreach (var hexed in role.Hexed)
            {
                var hexedPlayer = Utils.PlayerById(hexed);
                if (hexedPlayer.Data.IsDead || hexedPlayer.Data.Disconnected) role.Hexed.Remove(hexed);
            }

            // Set KillButton's cooldown
            HexButton.SetCoolDown(role.KillTimer(), CustomGameOptions.CovenKCD);
            role.HexBombButton.SetCoolDown(role.Cooldown, CustomGameOptions.CovenKCD);
            role.SabotageButton.SetCoolDown(0f, 1f);

            // Set the closest player for the Kill Button's targeting
            var notcoven = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !x.Is(Faction.Coven) && !role.Hexed.Contains(x.PlayerId) && !x.Data.IsDead)
                .ToList();

            if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
            else if (PlayerControl.LocalPlayer.IsLover() && CustomGameOptions.ImpLoverKillTeammate) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover()).ToList());
            else if (PlayerControl.LocalPlayer.IsLover()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Coven)).ToList());
            else Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Coven)).ToList());
            
            role.SabotageButton.graphic.color = Palette.EnabledColor;
            role.SabotageButton.graphic.material.SetFloat("_Desat", 0f);

            if (role.Hexed.Count > 0)
            {
                role.HexBombButton.graphic.color = Palette.EnabledColor;
                role.HexBombButton.graphic.material.SetFloat("_Desat", 0f);
            }
            else
            {
                role.HexBombButton.graphic.color = Palette.DisabledClear;
                role.HexBombButton.graphic.material.SetFloat("_Desat", 1f);
            }
        }
    }
}