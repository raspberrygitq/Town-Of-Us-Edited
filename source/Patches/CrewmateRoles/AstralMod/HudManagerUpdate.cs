using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.AstralMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudManagerUpdate
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateDissociateButton(__instance);
        }

        public static void UpdateDissociateButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Astral)) return;
            var ghostButton = __instance.KillButton;
            var role = Role.GetRole<Astral>(PlayerControl.LocalPlayer);

            ghostButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            ghostButton.buttonLabelText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            ghostButton.buttonLabelText.text = "Dissociate";
            ghostButton.buttonLabelText.SetOutlineColor(Patches.Colors.Astral);

            if (role.UsingGhost) ghostButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.GhostDuration);
            else ghostButton.SetCoolDown(role.AstralTimer(), CustomGameOptions.GhostCD);

            if (!role.UsingGhost && PlayerControl.LocalPlayer.Data.IsDead)
            {
                ghostButton.gameObject.SetActive(false);
            }

            var renderer = ghostButton.graphic;
            if (role.UsingGhost || !ghostButton.isCoolingDown)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                ghostButton.buttonLabelText.color = Palette.EnabledColor;
                ghostButton.buttonLabelText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
                ghostButton.buttonLabelText.color = Palette.DisabledClear;
                ghostButton.buttonLabelText.material.SetFloat("_Desat", 1f);
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
        public static class PlayerPhysicsUpdate
        {
            public static void Postfix(PlayerPhysics __instance)
            {
                if (PlayerControl.AllPlayerControls.Count <= 1) return;
                if (PlayerControl.LocalPlayer == null) return;
                if (PlayerControl.LocalPlayer.Data == null) return;
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Astral)) return;
                if (MeetingHud.Instance) return;
                var role = Role.GetRole<Astral>(PlayerControl.LocalPlayer);
                if (PlayerControl.LocalPlayer.Data.IsDead && !role.Enabled) return;

                if (__instance.myPlayer.Data.IsDead && __instance.myPlayer != PlayerControl.LocalPlayer)
                {
                    __instance.myPlayer.Visible = false;
                }
            }
        }
    }
}