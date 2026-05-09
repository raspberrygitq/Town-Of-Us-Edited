using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.ProsecutorMod
{
    [HarmonyPatch]
    public class ProseMeetingHudTimer
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
}
