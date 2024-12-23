using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.DeputyMod
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
            foreach (var role in Role.GetRoles(RoleEnum.Deputy))
            {
                var deputyRole = (Deputy)role;
                deputyRole.StartShooting = false;
            }
            return;
        }
    }
}
