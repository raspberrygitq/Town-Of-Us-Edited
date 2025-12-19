using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.NeutralRoles.DoomsayerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer)) return;
            var role = Role.GetRole<Doomsayer>(PlayerControl.LocalPlayer);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay) && !CustomGameOptions.DoomsayerCantObserve);
            __instance.KillButton.buttonLabelText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay) && !CustomGameOptions.DoomsayerCantObserve);

            if (role.ClosestPlayer != null)
            {
                __instance.KillButton.buttonLabelText.color = Palette.EnabledColor;
                __instance.KillButton.buttonLabelText.material.SetFloat("_Desat", 0f);
            }

            __instance.KillButton.buttonLabelText.color = Palette.DisabledClear;
            __instance.KillButton.buttonLabelText.material.SetFloat("_Desat", 0f);

            __instance.KillButton.SetCoolDown(role.ObserveTimer(), CustomGameOptions.ObserveCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
        }
    }
}