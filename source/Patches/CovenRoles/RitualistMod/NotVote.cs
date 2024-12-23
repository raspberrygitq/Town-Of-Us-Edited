using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CovenRoles.RitualistMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))] // BBFDNCCEJHI
    public static class VotingComplete
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Ritualist))
            {
                var ritualist = Role.GetRole<Ritualist>(PlayerControl.LocalPlayer);
                ShowHideButtonsRitualist.HideButtons(ritualist);
            }
        }
    }
}