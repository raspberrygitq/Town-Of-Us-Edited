using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.NeutraleRoles.ShifterMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudManagerUpdate
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateShiftButton(__instance);
        }

        public static void UpdateShiftButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Shifter)) return;

            var shiftButton = __instance.KillButton;
            var role = Role.GetRole<Shifter>(PlayerControl.LocalPlayer);

            shiftButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started
                    && PlayerControl.LocalPlayer.Is(RoleEnum.Shifter));
            shiftButton.graphic.sprite = TownOfUs.ShiftButton;
            HudManager.Instance.KillButton.buttonLabelText.gameObject.SetActive(false);
            shiftButton.SetCoolDown(role.ShiftTimer(), CustomGameOptions.ShiftCD);
            Utils.SetTarget(ref role.ClosestPlayer, shiftButton, float.NaN);
        }
    }
}
