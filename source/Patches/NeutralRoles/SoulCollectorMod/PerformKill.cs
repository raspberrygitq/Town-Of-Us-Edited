using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;
using AmongUs.GameOptions;

namespace TownOfUsEdited.NeutralRoles.SoulCollectorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.SoulCollector)) return true;
            var role = Role.GetRole<SoulCollector>(PlayerControl.LocalPlayer);
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];

            if (__instance == role.ReapButton)
            {
                if (role.Cooldown > 0) return false;
                if (role.ClosestPlayer == null) return false;
                if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                    PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
                if (interact[4] == true)
                {
                    role.ReapedPlayers.Add(role.ClosestPlayer.PlayerId);
                    Utils.Rpc(CustomRPC.Collect, role.Player.PlayerId, (byte)0, role.ClosestPlayer.PlayerId);
                }
                if (interact[0] == true)
                {
                    role.Cooldown = CustomGameOptions.ReapCd;
                    return false;
                }
                else if (interact[1] == true)
                {
                    role.Cooldown = CustomGameOptions.VestKCReset;
                    return false;
                }
                else if (interact[3] == true) return false;
                return false;
            }
            else
            {
                if (role.CurrentTarget == null)
                    return false;
                if (Vector2.Distance(role.CurrentTarget.gameObject.transform.position,
                    PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                var player = role.CurrentTarget.DeadPlayer;
                var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                if (!abilityUsed) return false;
                if (player.IsInfected() || role.Player.IsInfected())
                {
                    foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
                }
                role.SoulsCollected += 1;

                if (role.SoulsCollected >= CustomGameOptions.SoulsToWin)
                {
                    role.Wins();
                    Utils.Rpc(CustomRPC.SoulCollectorWin, role.Player.PlayerId);

                    if (!CustomGameOptions.NeutralEvilWinEndsGame)
                    {
                        KillButtonTarget.DontRevive = role.Player.PlayerId;
                        role.Player.Exiled();
                    }
                }
                UnityEngine.Object.Destroy(role.CurrentTarget.gameObject);
                role.Souls.Remove(role.CurrentTarget.gameObject);
                Utils.Rpc(CustomRPC.Collect, role.Player.PlayerId, (byte)1);
                role.CurrentTarget = null;
                return false;
            }
        }
    }
}
