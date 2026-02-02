using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CrewmateRoles.JailorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Jailor);
            if (!flag) return true;
            var role = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
            if (__instance != HudManager.Instance.KillButton) return true;
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            if (role.Cooldown > 0) return false;
            if (!__instance.enabled) return false;
            var maxDistance = LegacyGameOptions.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                var host = AmongUsClient.Instance.AmHost ? (byte)2 : (byte)0;
                if (AmongUsClient.Instance.AmHost) role.JailedPlayer = role.ClosestPlayer;
                Utils.Rpc(CustomRPC.SetJail, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
            }
            if (interact[0] == true)
            {
                role.Cooldown = CustomGameOptions.JailCD;
                return false;
            }
            else if (interact[1] == true)
            {
                role.Cooldown = CustomGameOptions.TempSaveCdReset;
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}
