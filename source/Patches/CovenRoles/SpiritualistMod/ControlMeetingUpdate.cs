using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CovenRoles.SpiritualistMod
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
            var spiritualists = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Spiritualist)).ToList();
            foreach (var spiritualist in spiritualists)
            {
                var role = Role.GetRole<Spiritualist>(spiritualist);
                if (role.ControlledPlayer != null)
                {
                    role.ControlledPlayer = null;
                }
                return;
            }
        }
    }
}
