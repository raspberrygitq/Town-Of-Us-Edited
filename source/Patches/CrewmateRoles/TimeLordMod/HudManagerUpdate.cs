using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.TimeLordMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            UpdateRewindButton(__instance);
        }

        public static void UpdateRewindButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.TimeLord)) return;
            var rewindButton = DestroyableSingleton<HudManager>.Instance.KillButton;

            var role = Role.GetRole<TimeLord>(PlayerControl.LocalPlayer);

            rewindButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            if (role.UsingRewind)
            {
                rewindButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.RewindDuration);
            }
            else
            {
                rewindButton.SetCoolDown(role.RewindTimer(), CustomGameOptions.RewindCooldown);
            }

            var renderer = rewindButton.graphic;
            if (role.UsingRewind || (!rewindButton.isCoolingDown && rewindButton.enabled))
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                return;
            }

            renderer.color = Palette.DisabledClear;
            renderer.material.SetFloat("_Desat", 1f);
        }
    }
}