using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.NeutralRoles.InfectiousMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static class MeetingHudUpdate
    {
        public static void Postfix(MeetingHud __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            var _role = Role.GetRole(localPlayer);
            if (_role?.RoleType != RoleEnum.Infectious) return;
            if (localPlayer.Data.IsDead) return;
            var role = (Infectious)_role;
            foreach (var state in __instance.playerStates)
            {
                var targetId = state.TargetPlayerId;
                var playerData = Utils.PlayerById(targetId)?.Data;
                var player = Utils.PlayerById(targetId);
                var playerRole = Role.GetRole(player);
                if (playerData == null || playerData.Disconnected || player.Is(RoleEnum.Infectious))
                {
                    role.Infected.Remove(targetId);
                    continue;
                }
                if (role.Infected.Contains(targetId) && role.Player.PlayerId != targetId)
                {
                    if (playerRole.InfectionState == 1) state.NameText.color = Color.yellow;
                    else if (playerRole.InfectionState == 2) state.NameText.color = Palette.Orange;
                    else if (playerRole.InfectionState == 3) state.NameText.color = Color.red;
                    else if (playerRole.InfectionState == 4) state.NameText.color = Color.black;
                }
            }
        }
    }
}