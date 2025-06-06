using System.Linq;
using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.CrusaderMod
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
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Crusader)) return;
            var crusadeButton = __instance.KillButton;

            var role = Role.GetRole<Crusader>(PlayerControl.LocalPlayer);

            crusadeButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            crusadeButton.SetCoolDown(role.CrusadeTimer(), CustomGameOptions.CrusadeCD);

            crusadeButton.buttonLabelText.text = "Crusade";
            crusadeButton.buttonLabelText.SetOutlineColor(role.Color);

            var notCrusaded = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => x != role.CrusadedPlayer && !x.Data.IsDead)
                .ToList();

            Utils.SetTarget(ref role.ClosestPlayer, crusadeButton, float.NaN, notCrusaded);

            var renderer = crusadeButton.graphic;

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

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    public class MeetingHudClose
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Crusader)) return;
            var crusRole = Role.GetRole<Crusader>(PlayerControl.LocalPlayer);
            crusRole.CrusadedPlayer = null;
            Utils.Rpc(CustomRPC.UpdateCrusade, PlayerControl.LocalPlayer.PlayerId);
        }
    }
}