using System.Linq;
using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.HelperMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            var player = PlayerControl.LocalPlayer;
            // Check if there is only one player or if local player is null or dead
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Helper)) return;

            // Get the Fighter role instance
            var role = Role.GetRole<Helper>(PlayerControl.LocalPlayer);

            // Check if the game state allows the KillButton to be active
            bool isKillButtonActive = __instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled;
            isKillButtonActive = isKillButtonActive && !MeetingHud.Instance && player.Data.IsDead;
            isKillButtonActive = isKillButtonActive && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
            AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay);

            // Set KillButton's visibility
            __instance.KillButton.gameObject.SetActive(isKillButtonActive);

            // Set KillButton's cooldown
            var alives = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();
            if (role.OnAlert) __instance.KillButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.HelperDuration);
            else __instance.KillButton.SetCoolDown(role.AlertTimer(), CustomGameOptions.HelperCD);
            if (!role.OnAlert) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, alives);

            if (role.ClosestPlayer != null || role.OnAlert)
            {
                __instance.KillButton.graphic.color = Palette.EnabledColor;
                __instance.KillButton.graphic.material.SetFloat("_Desat", 0f);
            }
            else
            {
                __instance.KillButton.graphic.color = Palette.DisabledClear;
                __instance.KillButton.graphic.material.SetFloat("_Desat", 1f);
            }
        }
    }
}