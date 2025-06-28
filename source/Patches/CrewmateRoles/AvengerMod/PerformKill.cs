using AmongUs.GameOptions;
using HarmonyLib;
using UnityEngine;
using TownOfUsEdited.Roles;
using TownOfUsEdited.CrewmateRoles.MedicMod;
using System.Linq;

namespace TownOfUsEdited.CrewmateRoles.AvengerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Avenger)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var maxDistance = LegacyGameOptions.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            var role = Role.GetRole<Avenger>(PlayerControl.LocalPlayer);

            if (__instance == role.AvengeButton)
            {
                if (role.CurrentTarget == null) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (Vector2.Distance(role.CurrentTarget.TruePosition,
                    PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                var playerId = role.CurrentTarget.ParentId;
                var player = Utils.PlayerById(playerId);
                foreach (var deadPlayer in Murder.KilledPlayers)
                {
                    if (deadPlayer.PlayerId == playerId)
                    {
                        var killerplayer = Utils.PlayerById(deadPlayer.KillerId);
                        if (deadPlayer.KillerId == PlayerControl.LocalPlayer.PlayerId ||
                        killerplayer.Data.IsDead || ((killerplayer.Is(Faction.Impostors) || killerplayer.Is(Faction.Madmates)) &&
                        PlayerControl.LocalPlayer.Is(Faction.Madmates)))
                        return false;
                    }
                }
                var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                if (!abilityUsed) return false;
                if (player.IsInfected() || role.Player.IsInfected())
                {
                    foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
                }
                foreach (var deadPlayer in Murder.KilledPlayers)
                {
                    if (deadPlayer.PlayerId == playerId)
                    {
                        if (!role.CurrentTarget.IsDouble())
                        {
                            var TargetKiller = Utils.PlayerById(deadPlayer.KillerId);
                            role.killer = TargetKiller;
                        }
                        else
                        {
                            var matches = Murder.KilledPlayers.ToArray().Where(x => x.KillerId == role.CurrentTarget.ParentId && x.isDoppel == true).ToList();
                            if (matches.Any())
                            {
                                foreach (var role2 in Role.GetRoles(RoleEnum.Doppelganger))
                                {
                                    var doppel = (Doppelganger)role2;
                                    role.killer = doppel.Player;
                                }
                            }
                        }
                    }
                }
                role.Avenging = true;
                return false;
            }
            else
            {
                if (role.ClosestPlayer == null) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (PlayerControl.LocalPlayer.IsControlled() && role.ClosestPlayer.Is(Faction.Coven))
                {
                    Utils.Interact(role.ClosestPlayer, PlayerControl.LocalPlayer, true);
                    return false;
                }
                if (!role.Avenging) return false;
                if (role.killer == null) return false;
                if (Vector2.Distance(role.killer.GetTruePosition(),
                    PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                if (role.ClosestPlayer != role.killer) return false;
                Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
                role.Avenging = false;
                role.killer = null;
                return false;
            }
        }
    }
}