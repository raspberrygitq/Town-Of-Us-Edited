using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Modifiers.AssassinMod;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.HunterMod
{
    public static class Retribution
    {
        public static PlayerControl LastVoted = null;
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CastVote))]
    internal class CastVote
    {
        private static void Postfix(MeetingHud __instance, [HarmonyArgument(0)] byte srcPlayerId, [HarmonyArgument(1)] byte suspectPlayerId)
        {
            var votingPlayer = Utils.PlayerById(srcPlayerId);
            var suspectPlayer = Utils.PlayerById(suspectPlayerId);
            if (!suspectPlayer.Is(RoleEnum.Hunter)) return;
            Retribution.LastVoted = votingPlayer;
        }
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
    internal class MeetingExiledEnd
    {
        private static void Postfix(ExileController __instance)
        {
            var exiled = __instance.initData?.networkedPlayer;
            if (exiled == null) return;
            var player = exiled.Object;

            if (player.Is(RoleEnum.Hunter) && CustomGameOptions.RetributionOnVote)
            {
                var role = Role.GetRole<Hunter>(player);
                foreach (var exeRole in Role.AllRoles.Where(x => x.RoleType == RoleEnum.Executioner))
                {
                    var exe = (Executioner)exeRole;
                    if (exe.target == player) return;
                }
                if (Retribution.LastVoted != null && Retribution.LastVoted.PlayerId != player.PlayerId)
                {
                    Utils.Rpc(CustomRPC.Retribution, Retribution.LastVoted.PlayerId, player.PlayerId);
                    AssassinKill.MurderPlayer(Retribution.LastVoted, player);
                    role.UsedRetribution = true;
                }
            }
            Retribution.LastVoted = null;
        }
    }
}