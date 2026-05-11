using HarmonyLib;
using TownOfUsEdited.Roles;
using TownOfUsEdited.Roles.Modifiers;
using Assassin = TownOfUsEdited.Roles.Modifiers.Assassin;

namespace TownOfUsEdited.Patches;

public static class MeetingHudTimerPatch
{
    [HarmonyPatch]
    public static class ProseMeetingHud
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        [HarmonyPostfix]
        public static void Postfix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Prosecutor)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (__instance.TimerText.text.Contains("Who Is The Impostor?")) return;

            var role = Role.GetRole<Prosecutor>(PlayerControl.LocalPlayer);
            if (role == null) return;

            __instance.TimerText.text = __instance.TimerText.text + "\n" + $"Prosecutions Remaining: {role.ProsecutesRemaining} / {CustomGameOptions.MaxProsecutes}";
        }
    }

    [HarmonyPatch]
    public static class VigiMeetingHud
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

    [HarmonyPatch]
    public static class AssassinMeetingHud
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        [HarmonyPostfix]
        public static void Postfix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(AbilityEnum.Assassin)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (__instance.TimerText.text.Contains("Who Is The Impostor?")) return;

            var ability = Ability.GetAbility<Assassin>(PlayerControl.LocalPlayer);
            if (ability == null) return;

            __instance.TimerText.text = __instance.TimerText.text + "\n" + $"Guesses Remaining: {ability.RemainingKills} / {CustomGameOptions.AssassinKills}";

            var doubleShot = Modifier.GetModifier<DoubleShot>(PlayerControl.LocalPlayer);
            if (doubleShot != null)
            {
                if (!doubleShot.LifeUsed)
                {
                    __instance.TimerText.text = __instance.TimerText.text + " | Double Shot Available";
                }
                else
                {
                    __instance.TimerText.text = __instance.TimerText.text + " | Double Shot Used";
                }
            }
        }
    }

    [HarmonyPatch]
    public static class DoomMeetingHudTimer
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

    [HarmonyPatch]
    public static class RitualistMeetingHud
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
