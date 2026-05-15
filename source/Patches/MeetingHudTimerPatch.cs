using HarmonyLib;
using TownOfUsEdited.Roles;
using TownOfUsEdited.Roles.Modifiers;
using Assassin = TownOfUsEdited.Roles.Modifiers.Assassin;

namespace TownOfUsEdited.Patches;

public static class MeetingHudTimerPatch
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

    [HarmonyPatch]
    public static class DoomMeetingHudTimer
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
            if (role == null) return;
            var remainingKillsText = role.RemainingKills == 0 ? "No Guesses Remaining" : $"Guesses Remaining: {role.RemainingKills} / {CustomGameOptions.RitualistKills}";
            __instance.TimerText.text = __instance.TimerText.text + "\n" + $"{Palette.White.ToTextColor()}{remainingKillsText}</color>";
        }
    }
}
