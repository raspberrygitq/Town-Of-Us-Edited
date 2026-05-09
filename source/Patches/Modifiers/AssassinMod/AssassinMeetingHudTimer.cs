using HarmonyLib;
using TownOfUsEdited.Roles.Modifiers;

namespace TownOfUsEdited.Modifiers.AssassinMod
{
    public class AssassinMeetingHudTimer
    {
        [HarmonyPatch]
        public static class MeetingHudUpdate
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

                __instance.TimerText.text = __instance.TimerText.text + "\n" + $"Remaining Kill: {ability.RemainingKills} / {CustomGameOptions.AssassinKills}";

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
    }
}
