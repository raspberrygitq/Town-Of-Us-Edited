using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CovenRoles.RitualistMod;

public static class RituMeetingTimerPatch
{
    [HarmonyPatch]
    public static class RitualistMeetingHud
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.UpdateTimerText))]
        [HarmonyPostfix]
        public static void Postfix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Ritualist)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (__instance.TimerText.text.Contains("Who Is The Impostor?")) return;
            var role = Role.GetRole<Ritualist>(PlayerControl.LocalPlayer);
            var remainingKillsText = role.RemainingKills == 0 ? "No Guesses Remaining" : $"Guesses Remaining: {role.RemainingKills} / {CustomGameOptions.RitualistKills}";
            __instance.TimerText.text = __instance.TimerText.text + "\n" + $"{Palette.White.ToTextColor()}{remainingKillsText}</color>";
        }
    }
}
