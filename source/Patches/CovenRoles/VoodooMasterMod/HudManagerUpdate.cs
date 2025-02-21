using System.Linq;
using HarmonyLib;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CovenRoles.VoodooMasterMod
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
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.VoodooMaster)) return;

            var role = Role.GetRole<VoodooMaster>(PlayerControl.LocalPlayer);

            if (role.VoodooButton == null)
            {
                role.VoodooButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.VoodooButton.graphic.enabled = true;
                role.VoodooButton.gameObject.SetActive(false);
            }

            role.VoodooButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            role.VoodooButton.graphic.sprite = TownOfUsEdited.Voodoo;
            role.VoodooButton.transform.localPosition = new Vector3(-2f, 1f, 0f);

            foreach (var player2 in PlayerControl.AllPlayerControls)
            {
                if (player2 == role.VoodooPlayer && !player2.Data.IsDead && !player2.Data.Disconnected) player2.nameText().color = Color.black;
            }

            // Set Button's cooldown
            role.VoodooButton.SetCoolDown(role.KillCooldown, CustomGameOptions.CovenKCD);

            if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayer, role.VoodooButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsVoodoo()).ToList());
            else if (PlayerControl.LocalPlayer.IsLover() && CustomGameOptions.ImpLoverKillTeammate) Utils.SetTarget(ref role.ClosestPlayer, role.VoodooButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.IsVoodoo()).ToList());
            else if (PlayerControl.LocalPlayer.IsLover()) Utils.SetTarget(ref role.ClosestPlayer, role.VoodooButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Coven) && !x.IsVoodoo()).ToList());
            else Utils.SetTarget(ref role.ClosestPlayer, role.VoodooButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Coven) && !x.IsVoodoo()).ToList());
        }
    }
}