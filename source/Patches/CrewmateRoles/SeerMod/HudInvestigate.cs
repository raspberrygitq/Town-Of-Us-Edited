using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.SeerMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudInvestigate
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateInvButton(__instance);
            UpdateRevealButton(__instance);
        }

        public static void UpdateInvButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Seer)) return;
            if (CustomGameOptions.GameMode == GameMode.Werewolf) return;
            var investigateButton = __instance.KillButton;

            var role = Role.GetRole<Seer>(PlayerControl.LocalPlayer);

            investigateButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            investigateButton.buttonLabelText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay);

            investigateButton.buttonLabelText.text = "Reveal";
            investigateButton.buttonLabelText.SetOutlineColor(Patches.Colors.Seer);

            investigateButton.SetCoolDown(role.SeerTimer(), CustomGameOptions.SeerCd);

            var notInvestigated = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !role.Investigated.Contains(x.PlayerId))
                .ToList();

            Utils.SetTarget(ref role.ClosestPlayer, investigateButton, float.NaN, notInvestigated);

            var renderer = investigateButton.graphic;
            var revealText = investigateButton.buttonLabelText;
            if (role.ClosestPlayer != null)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                revealText.color = Palette.EnabledColor;
                revealText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
                revealText.color = Palette.DisabledClear;
                revealText.material.SetFloat("_Desat", 0f);
            }
        }

        public static void UpdateRevealButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Seer)) return;
            if (CustomGameOptions.GameMode != GameMode.Werewolf) return;
            var revealButton = __instance.KillButton;

            var role = Role.GetRole<Seer>(PlayerControl.LocalPlayer);

            revealButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay)
                    && !role.UsedReveal);

            var notInvestigated = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !role.Revealed.Contains(x.PlayerId))
                .ToList();

            Utils.SetTarget(ref role.ClosestPlayer, revealButton, float.NaN, notInvestigated);

            var renderer = revealButton.graphic;

            if (role.ClosestPlayer != null)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
        }
    }
}
