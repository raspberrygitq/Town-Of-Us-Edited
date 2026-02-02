using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.JailorMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class UpdateJailButton
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jailor)) return;
            var jailButton = __instance.KillButton;

            var role = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);

            jailButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            jailButton.buttonLabelText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            jailButton.buttonLabelText.text = "Jail";
            jailButton.buttonLabelText.SetOutlineColor(Patches.Colors.Jailor);

            if (role.CanJail)
            {
                jailButton.SetCoolDown(role.JailTimer(), CustomGameOptions.JailCD);
                Utils.SetTarget(ref role.ClosestPlayer, jailButton, float.NaN);
            }
            else
            {
                jailButton.SetCoolDown(0f, CustomGameOptions.JailCD);
                jailButton.SetTarget(null);
            }

            var notJailed = PlayerControl.AllPlayerControls.ToArray().Where(x => x != role.JailedPlayer).ToList();

            if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, notJailed);
            else if (role.Player.IsLover() && role.Player.Is(Faction.Madmates) && !CustomGameOptions.MadmateKillEachOther) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Impostors) && x != role.JailedPlayer).ToList());
            else if (role.Player.Is(Faction.Madmates) && !CustomGameOptions.MadmateKillEachOther) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Impostors) && x != role.JailedPlayer).ToList());
            else if (role.Player.IsLover()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && x != role.JailedPlayer).ToList());
            else Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, notJailed);

            if (role.ClosestPlayer != null && role.CanJail)
            {
                jailButton.graphic.color = Palette.EnabledColor;
                jailButton.graphic.material.SetFloat("_Desat", 0f);
                jailButton.buttonLabelText.color = Palette.EnabledColor;
                jailButton.buttonLabelText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                jailButton.graphic.color = Palette.DisabledClear;
                jailButton.graphic.material.SetFloat("_Desat", 1f);
                jailButton.buttonLabelText.color = Palette.DisabledClear;
                jailButton.buttonLabelText.material.SetFloat("_Desat", 1f);
            }
        }
    }
}