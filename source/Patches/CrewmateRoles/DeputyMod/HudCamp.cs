using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.DeputyMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudCamp
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Deputy)) return;
            var campButton = __instance.KillButton;

            var role = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);

            campButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            campButton.buttonLabelText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            campButton.SetCoolDown(0f, 1f);

            if (role.Camping == null && role.CampedThisRound == false) Utils.SetTarget(ref role.ClosestPlayer, campButton, float.NaN);
            else campButton.SetTarget(null);
        }
    }
}
