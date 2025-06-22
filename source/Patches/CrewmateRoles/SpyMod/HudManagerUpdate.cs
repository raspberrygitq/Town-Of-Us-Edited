using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.SpyMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudManagerUpdate
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Spy)) return;

            var role = Role.GetRole<Spy>(PlayerControl.LocalPlayer);
            var AdminText = __instance.KillButton.buttonLabelText;

            __instance.KillButton.SetCoolDown(0f, 10f);
            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started && CustomGameOptions.SpyHasPortableAdmin);
            __instance.KillButton.graphic.sprite = HudManager.Instance.UseButton.fastUseSettings[ImageNames.PolusAdminButton].Image;

            AdminText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            if (PlayerControl.LocalPlayer.Data.IsDead) return;

            var renderer = __instance.KillButton.graphic;
            AdminText.color = Palette.EnabledColor;
            AdminText.material.SetFloat("_Desat", 0f);
            if (!CamouflageUnCamouflage.CommsEnabled)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                return;
            } 
            else 
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
        }
    }
}