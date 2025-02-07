using HarmonyLib;
using TownOfUs.Roles;
using System.Linq;

namespace TownOfUs.CrewmateRoles.LookoutMod
{

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    class StartMeetingPatch
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] NetworkedPlayerInfo meetingTarget)
        {
            if (__instance == null)
            {
                return;
            }
            var lookouts = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Lookout)).ToList();
            foreach (var lookout in lookouts)
            {
                var role = Role.GetRole<Lookout>(lookout);
                if (lookout != null && role.WatchedPlayer != null)
                {
                    if (CustomGameOptions.WatchedKnows) Utils.Rpc(CustomRPC.StopWatch, role.Player.PlayerId);
                    role.StopWatching();
                }
                return;
            }
        }
    }
}
