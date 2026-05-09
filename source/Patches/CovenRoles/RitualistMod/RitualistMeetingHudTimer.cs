using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CovenRoles.RitualistMod
{
    [HarmonyPatch]
    public class RitualistMeetingHudTimer
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        [HarmonyPostfix]
        public static void Postfix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Ritualist)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (__instance.TimerText.text.Contains("Who Is The Impostor?")) return;

            var role = Role.GetRole<Ritualist>(PlayerControl.LocalPlayer);
            if (role == null) return;

            __instance.TimerText.text = __instance.TimerText.text + "\n" + $"Remaining Kill: {role.RemainingKills} / {CustomGameOptions.RitualistKills}";
        }
    }
}
