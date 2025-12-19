using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CrewmateRoles.CrusaderMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Crusader);
            if (!flag) return true;
            var role = Role.GetRole<Crusader>(PlayerControl.LocalPlayer);
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
                role.CrusadedPlayer = role.ClosestPlayer;
                Utils.Rpc(CustomRPC.SetCrusade, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
                role.Cooldown = CustomGameOptions.CrusadeCD;
            }
            if (interact[0] == true)
            {
                role.Cooldown = CustomGameOptions.CrusadeCD;
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
