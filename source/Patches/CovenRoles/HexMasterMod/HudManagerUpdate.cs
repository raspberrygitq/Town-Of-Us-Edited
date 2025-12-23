using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Patches;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CovenRoles.HexMasterMod
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

            if (role.HexBombButton == null)
            {
                role.HexBombButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.HexBombButton.graphic.enabled = true;
                role.HexBombButton.gameObject.SetActive(false);
            }

            HexButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            HexButton.buttonLabelText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            HexButton.buttonLabelText.text = "Hex";
            HexButton.buttonLabelText.SetOutlineColor(Colors.Coven);

            role.HexBombButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.HexBombButton.buttonLabelText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.HexBombButton.buttonLabelText.text = "Hex Bomb";
            role.HexBombButton.buttonLabelText.SetOutlineColor(Colors.Coven);

            role.HexBombButton.graphic.sprite = TownOfUsEdited.HexBomb;
            role.HexBombButton.transform.localPosition = new Vector3(-2f, 1f, 0f);

            foreach (var hexed in role.Hexed)
            {
                var hexedPlayer = Utils.PlayerById(hexed);
                if (hexedPlayer.Data.IsDead || hexedPlayer.Data.Disconnected) role.Hexed.Remove(hexed);
            }

            // Set KillButton's cooldown
            HexButton.SetCoolDown(role.KillTimer(), CustomGameOptions.CovenKCD);
            role.HexBombButton.SetCoolDown(role.Cooldown, CustomGameOptions.CovenKCD);

            // Set the closest player for the Kill Button's targeting
            var notcoven = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !x.Is(Faction.Coven) && !x.IsHexed() && !x.Data.IsDead)
                .ToList();

            if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsHexed()).ToList());
            else if (PlayerControl.LocalPlayer.IsLover() && CustomGameOptions.ImpLoverKillTeammate) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.IsHexed()).ToList());
            else if (PlayerControl.LocalPlayer.IsLover()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Coven) && !x.IsHexed()).ToList());
            else Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, notcoven);

            if (role.Hexed.Count > 0 && !role.coolingDown)
            {
                role.HexBombButton.buttonLabelText.color = Palette.EnabledColor;
                role.HexBombButton.buttonLabelText.material.SetFloat("_Desat", 0f);
                role.HexBombButton.graphic.color = Palette.EnabledColor;
                role.HexBombButton.graphic.material.SetFloat("_Desat", 0f);
            }
            else
            {
                role.HexBombButton.buttonLabelText.color = Palette.DisabledClear;
                role.HexBombButton.buttonLabelText.material.SetFloat("_Desat", 1f);
                role.HexBombButton.graphic.color = Palette.DisabledClear;
                role.HexBombButton.graphic.material.SetFloat("_Desat", 1f);
            }

            var labelrender = HexButton.buttonLabelText;
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