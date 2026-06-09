using HarmonyLib;

namespace TownOfUsEdited.Modifiers.AssassinMod;

public static class AssassinMeetingTimerPatch
{
    [HarmonyPatch]
    public static class AssassinMeetingHud
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.UpdateTimerText))]
        [HarmonyPostfix]
        public static void Postfix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(AbilityEnum.Assassin)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (__instance.TimerText.text.Contains("Who Is The Impostor?")) return;
            var ability = Ability.GetAbility<Assassin>(PlayerControl.LocalPlayer);
            var remainingKillsText = ability.RemainingKills == 0 ? "No Guesses Remaining" : $"Guesses Remaining: {ability.RemainingKills} / {CustomGameOptions.AssassinKills}";
            __instance.TimerText.text = __instance.TimerText.text + "\n" + $"{Palette.White.ToTextColor()}{remainingKillsText}</color>";
            var doubleShot = Modifier.GetModifier<DoubleShot>(PlayerControl.LocalPlayer);
            if (doubleShot != null)
            {
                if (!doubleShot.LifeUsed)
                {
                    __instance.TimerText.text = __instance.TimerText.text + $"{Palette.White.ToTextColor()} | Double Shot Available</color>";
                }
                else
                {
                    __instance.TimerText.text = __instance.TimerText.text + $"{Palette.White.ToTextColor()} | Double Shot Used</color>";
                }
            }
        }
    }
}
