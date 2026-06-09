using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.JailorMod;

public static class JailorMeetingTimerPatch
{
    [HarmonyPatch]
    public static class JailorMeetingHud
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.UpdateTimerText))]
        [HarmonyPostfix]
        public static void Postfix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jailor)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (__instance.TimerText.text.Contains("Who Is The Impostor?")) return;
            var role = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
            var executesText = role.Executes == 0 ? "No Executes Remaining" : !role.CanJail ? "Cannot Jail Players" : $"Executes Remaining: {role.Executes} / {CustomGameOptions.MaxExecutes}";
            __instance.TimerText.text = __instance.TimerText.text + "\n" + $"{Palette.White.ToTextColor()}{executesText}</color>";
        }
    }
}
