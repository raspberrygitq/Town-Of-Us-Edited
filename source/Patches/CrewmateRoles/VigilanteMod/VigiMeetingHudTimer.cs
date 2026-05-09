using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.VigilanteMod
{
    [HarmonyPatch]
    public class VigiMeetingHudTimer
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        [HarmonyPostfix]
        public static void Postfix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (__instance.TimerText.text.Contains("Who Is The Impostor?")) return;

            var role = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer);
            if (role == null) return;

            var safeShotsText = role.SafeShots == 0 ? "No Safe Shots" : $"Safe Shots Available: {role.SafeShots} / {CustomGameOptions.VigilanteSafeShots}";

            __instance.TimerText.text = __instance.TimerText.text + "\n" + $"Guesses Remaining: {role.RemainingKills} / {CustomGameOptions.VigilanteKills} | {safeShotsText}";
        }
    }
}