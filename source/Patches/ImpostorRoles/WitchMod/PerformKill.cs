using HarmonyLib;
using Reactor.Utilities;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;
using TownOfUs.ImpostorRoles.ImpostorMod;

namespace TownOfUs.ImpostorRoles.JanitorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill

    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Witch);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Witch>(PlayerControl.LocalPlayer);

            if (__instance == role.SpellButton)
            {
                var flag2 = __instance.isCoolingDown;
                if (flag2) return false;
                if (!__instance.enabled) return false;
                if (role.ClosestPlayer == null) return false;
                var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
                if (interact[4] == true)
                {
                    role.CursedList.Add(role.ClosestPlayer.PlayerId);
                    Utils.Rpc(CustomRPC.Curse, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
                    Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                }
                if (interact[0] == true)
                {
                    Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                    return false;
                }
                else if (interact[1] == true)
                {
                    Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = CustomGameOptions.ProtectKCReset + 0.01f;
                    return false;
                }
                else if (interact[3] == true) return false;
                return false;
            }

            return true;
        }
    }
}