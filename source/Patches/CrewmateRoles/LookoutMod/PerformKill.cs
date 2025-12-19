using HarmonyLib;
using Reactor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using TownOfUsEdited.Patches.NeutralRoles;
using TownOfUsEdited.Roles;
using Object = UnityEngine.Object;

namespace TownOfUsEdited.CrewmateRoles.LookoutMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Lookout);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Lookout>(PlayerControl.LocalPlayer);
            if (role.Cooldown > 0) return false;
            if (role.IsWatching) return false;
            if (!__instance.enabled) return false;
            var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
            if (!abilityUsed) return false;
            List<byte> watchablePlayers = new List<byte>();
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!player.Data.Disconnected && player != PlayerControl.LocalPlayer)
                {
                    if (!player.Data.IsDead) watchablePlayers.Add(player.PlayerId);
                    else
                    {
                        foreach (var body in Object.FindObjectsOfType<DeadBody>())
                        {
                            if (body.ParentId == player.PlayerId) watchablePlayers.Add(player.PlayerId);
                        }
                    }
                }
            }
            byte[] watchablePlayersIds = watchablePlayers.ToArray();
            var pk = new PlayerMenu((x) =>
            {
                role.WatchedPlayer = x;
                role.TimeRemaining = CustomGameOptions.WatchDuration;
                role.StartWatching();
                PlayerControl.LocalPlayer.NetTransform.Halt();
                if (CustomGameOptions.WatchedKnows) Utils.Rpc(CustomRPC.StartWatch, PlayerControl.LocalPlayer.PlayerId, role.WatchedPlayer.PlayerId);
                role.IsWatching = true;
            }, (y) =>
            {
                return watchablePlayersIds.Contains(y.PlayerId);
            });
            Coroutines.Start(pk.Open(0f, true));
            return false;
        }
    }
}