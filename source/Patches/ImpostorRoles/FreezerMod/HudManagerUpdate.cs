using System.Linq;
using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.FreezerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            var player = PlayerControl.LocalPlayer;
            // Check if there is only one player or if local player is null or dead
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Freezer)) return;

            // Get the Freezer role instance
            var role = Role.GetRole<Freezer>(PlayerControl.LocalPlayer);

            if (role.FreezeButton == null)
            {
                role.FreezeButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.FreezeButton.graphic.enabled = true;
                role.FreezeButton.gameObject.SetActive(false);
            }

            role.FreezeButton.graphic.sprite = TownOfUs.Freeze;
            role.FreezeButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            var position = __instance.KillButton.transform.localPosition;
            role.FreezeButton.transform.localPosition = new Vector3(position.x,
                position.y, position.z);

            // Set KillButton's cooldown
            var notimps = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Data.IsImpostor() && !x.Is(Faction.Madmates)).ToList();
            if (role.Freezing) role.FreezeButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.BlindDuration);
            else role.FreezeButton.SetCoolDown(role.FreezeTimer(), CustomGameOptions.BlindCD);
            Utils.SetTarget(ref role.ClosestPlayer, role.FreezeButton, float.NaN, notimps);

            return;
            
        }
    }
}