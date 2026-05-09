using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.NeutralRoles.DoomsayerMod
{
    [HarmonyPatch]
    public class DoomMeetingHudTimer
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        [HarmonyPostfix]
        public static void Postfix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (__instance.TimerText.text.Contains("Who Is The Impostor?")) return;

            var role = Role.GetRole<Doomsayer>(PlayerControl.LocalPlayer);
            if (role == null) return;

            __instance.TimerText.text = __instance.TimerText.text + "\n" + $"Guesses To Win: {role.GuessedCorrectly} / {CustomGameOptions.DoomsayerGuessesToWin}";
        }
    }
}