using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.VigilanteMod;

public static class VigiMeetingTimerPatch
{
    [HarmonyPatch]
    public static class VigiMeetingHud
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.UpdateTimerText))]
        [HarmonyPostfix]
        public static void Postfix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (__instance.TimerText.text.Contains("Who Is The Impostor?")) return;
            var role = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer);
            var safeShotsText = role.SafeShots == 0 ? "No Safe Shots" : $"Safe Shots Available: {role.SafeShots} / {CustomGameOptions.VigilanteSafeShots}";
            var remainingKillsText = role.RemainingKills == 0 ? "No Guesses Remaining" : $"Guesses Remaining: {role.RemainingKills} / {CustomGameOptions.VigilanteKills}";
            __instance.TimerText.text = __instance.TimerText.text + "\n" + $"{Palette.White.ToTextColor()}{remainingKillsText} | {safeShotsText}</color>";
        }
    }
}
