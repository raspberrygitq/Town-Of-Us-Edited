using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.ProsecutorMod;

public static class ProseMeetingTimerPatch
{
    [HarmonyPatch]
    public static class ProseMeetingHud
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.UpdateTimerText))]
        [HarmonyPostfix]
        public static void Postfix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Prosecutor)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (__instance.TimerText.text.Contains("Who Is The Impostor?")) return;
            var role = Role.GetRole<Prosecutor>(PlayerControl.LocalPlayer);
            var prosecuteText = role.ProsecutesRemaining == 0 ? "No Prosecutions Remaining" : $"Prosecutions Remaining: {role.ProsecutesRemaining} / {CustomGameOptions.MaxProsecutes}";
            __instance.TimerText.text = __instance.TimerText.text + "\n" + $"{Palette.White.ToTextColor()}{prosecuteText}</color>";
        }
    }
}
