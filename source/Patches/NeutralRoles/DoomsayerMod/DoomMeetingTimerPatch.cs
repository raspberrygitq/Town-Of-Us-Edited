using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.NeutralRoles.DoomsayerMod;

public static class DoomMeetingTimerPatch
{
    [HarmonyPatch]
    public static class DoomMeetingHud
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.UpdateTimerText))]
        [HarmonyPostfix]
        public static void Postfix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (__instance.TimerText.text.Contains("Who Is The Impostor?")) return;
            var role = Role.GetRole<Doomsayer>(PlayerControl.LocalPlayer);
            __instance.TimerText.text = __instance.TimerText.text + "\n" + $"{Palette.White.ToTextColor()}Guesses To Win: {role.GuessedCorrectly} / {CustomGameOptions.DoomsayerGuessesToWin}</color>";
        }
    }
}
