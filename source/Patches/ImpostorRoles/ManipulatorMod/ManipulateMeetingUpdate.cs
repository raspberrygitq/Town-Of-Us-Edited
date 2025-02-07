using HarmonyLib;
using TownOfUs.Roles;
using System.Linq;

namespace TownOfUs.ImpostorRoles.ManipulatorMod
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
            var manipulators = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Manipulator)).ToList();
            foreach (var manipulator in manipulators)
            {
                var role = Role.GetRole<Manipulator>(manipulator);
                if (manipulator != null && role.ManipulatedPlayer != null)
                {
                    Utils.Rpc(CustomRPC.SetManipulateOff, role.Player.PlayerId);
                    role.StopManipulation();
                }
                return;
            }
        }
    }
}
