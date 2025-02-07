using System.Collections;
using HarmonyLib;
using Reactor.Utilities;
using TownOfUs.Patches.ImpostorRoles.ManipulatorMod;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.ManipulatorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Manipulator);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Manipulator>(PlayerControl.LocalPlayer);
            if (__instance == role.ManipulateButton)
            {
                if (role.ManipulatedPlayer == null)
                {
                    var flag2 = __instance.isCoolingDown;
                    if (flag2) return false;
                    if (!__instance.enabled) return false;
                    if (role == null)
                        return false;
                    if (role.ClosestPlayer == null)
                        return false;
                    var playerId = role.ClosestPlayer.PlayerId;
                    var player = Utils.PlayerById(playerId);
                    var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                    if (!abilityUsed) return false;
                    if (player.IsInfected() || role.Player.IsInfected())
                    {
                        foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
                    }
                    var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
                    if (interact[4] == true)
                    {
                        role.ManipulatedPlayer = role.ClosestPlayer;

                        Utils.Rpc(CustomRPC.SetManipulate, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);

                        role.Cooldown = 1f;
                    }
                }
                else
                {
                    var flag2 = __instance.isCoolingDown;
                    if (flag2) return false;
                    if (!__instance.enabled) return false;
                    if (role == null)
                        return false;
                    var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                    if (!abilityUsed) return false;
                    role.IsManipulating = true;

                    Utils.Rpc(CustomRPC.StartManipulate, PlayerControl.LocalPlayer.PlayerId);

                    role.TimeRemaining = CustomGameOptions.ManipulationDuration;
                    role.StartManipulation();
                    PlayerControl.LocalPlayer.NetTransform.Halt();
                }
            }
            return false;
        }
    }
}