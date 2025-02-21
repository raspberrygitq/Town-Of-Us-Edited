using HarmonyLib;
using TownOfUsEdited.Roles;
using System.Linq;

namespace TownOfUsEdited.CrewmateRoles.AvengerMod
{

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    class StartMeetingPatch
    {
        public static void Prefix(PlayerControl __instance)
        {
            if (__instance == null)
            {
                return;
            }
            var avengers = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Avenger)).ToList();
            foreach (var avenger in avengers)
            {
                var role = Role.GetRole<Avenger>(avenger);
                if (role.killer != null)
                {
                    role.killer = null;
                    role.Avenging = false;
                }
                return;
            }
        }
    }
}
