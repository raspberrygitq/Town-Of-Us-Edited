using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.GuardianMod
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
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Guardian)) return;

            // Get the Guardian role instance
            var role = Role.GetRole<Guardian>(PlayerControl.LocalPlayer);

            // Check if the game state allows the KillButton to be active
            bool isKillButtonActive = __instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled;
            isKillButtonActive = isKillButtonActive && !MeetingHud.Instance && player.Data.IsDead;
            isKillButtonActive = isKillButtonActive && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
            AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay);

            // Set KillButton's visibility
            __instance.KillButton.gameObject.SetActive(isKillButtonActive);
            __instance.KillButton.buttonLabelText.gameObject.SetActive(isKillButtonActive);

            __instance.KillButton.buttonLabelText.text = "Protect";
            __instance.KillButton.buttonLabelText.SetOutlineColor(Patches.Colors.Guardian);

            // Set KillButton's cooldown
            var alives = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();
            if (role.Guarding) __instance.KillButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.GuardDuration);
            else __instance.KillButton.SetCoolDown(role.GuardTimer(), CustomGameOptions.GuardCD);
            if (!role.Guarding) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, alives);

            var labelrender = __instance.KillButton.buttonLabelText;
            if (role.ClosestPlayer != null && !role.Guarding && !role.coolingDown)
            {
                __instance.KillButton.graphic.color = Palette.EnabledColor;
                __instance.KillButton.graphic.material.SetFloat("_Desat", 0f);
                labelrender.color = Palette.EnabledColor;
                labelrender.material.SetFloat("_Desat", 0f);
            }
            else
            {
                __instance.KillButton.graphic.color = Palette.DisabledClear;
                __instance.KillButton.graphic.material.SetFloat("_Desat", 1f);
                labelrender.color = Palette.DisabledClear;
                labelrender.material.SetFloat("_Desat", 1f);
            }
        }
    }
}