using HarmonyLib;
using TownOfUs.Roles;
using System.Linq;

namespace TownOfUs.ImpostorRoles.ConjurerMod
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
            var conjurers = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Conjurer)).ToList();
            foreach (var conjurer in conjurers)
            {
                var role = Role.GetRole<Conjurer>(conjurer);
                if (conjurer != role.CursedPlayer && role.CursedPlayer != null)
                {
                    role.CursedPlayer = null;
                }
                return;
            }
        }
    }
}
