using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.MedicMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HUDProtect
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateProtectButton(__instance);
        }

        public static void UpdateProtectButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Medic)) return;

            var protectButton = __instance.KillButton;
            var shieldText = __instance.KillButton.buttonLabelText;
            var role = Role.GetRole<Medic>(PlayerControl.LocalPlayer);

            protectButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            shieldText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            protectButton.SetCoolDown(role.StartTimer(), 10f);

            if (role.ShieldedPlayer != null) return;
            Utils.SetTarget(ref role.ClosestPlayer, protectButton);
        }
    }
}
