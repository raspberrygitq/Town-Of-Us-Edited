using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.TimeLordMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class RewindStopRewind
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.TimeLord))
            {
                var tl = (TimeLord)role;
                if (tl.UsingRewind)
                    tl.Rewind();
                else if (tl.Rewinding)
                {
                    tl.StopRewind();
                    PerformRewind.StopRewind(tl);
                }
            }
        }
    }
}