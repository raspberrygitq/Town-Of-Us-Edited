using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.SheriffMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDKill
    {
        private static KillButton KillButton;

        public static void Postfix(HudManager __instance)
        {
            KillButton = __instance.KillButton;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            var flag7 = PlayerControl.AllPlayerControls.Count > 1;
            if (!flag7) return;
            var flag8 = PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff);
            if (flag8)
            {
                var role = Role.GetRole<Sheriff>(PlayerControl.LocalPlayer);
                KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
                KillButton.buttonLabelText.text = "Shoot";
                KillButton.SetCoolDown(role.SheriffKillTimer(), CustomGameOptions.SheriffKillCd);

                if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
                else if (role.Player.IsLover() && role.Player.Is(Faction.Madmates) && !CustomGameOptions.MadmateKillEachOther) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Impostors)).ToList());
                else if (role.Player.Is(Faction.Madmates) && !CustomGameOptions.MadmateKillEachOther) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Impostors)).ToList());
                else if (role.Player.IsLover()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover()).ToList());
                else Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
            }
        }

        public static void ImpKillTarget(KillButton killButton)
        {
            PlayerControl target = null;

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Manipulator) && Role.GetRole<Manipulator>(PlayerControl.LocalPlayer).UsingManipulation)
            {
                var role = Role.GetRole<Manipulator>(PlayerControl.LocalPlayer);
                if (!killButton.isActiveAndEnabled) target = null;
                else if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTargetPlayer(ref target, killButton, role.ManipulatedPlayer);
                else if (PlayerControl.LocalPlayer.IsLover() && CustomGameOptions.ImpLoverKillTeammate) Utils.SetTargetPlayer(ref target, killButton, role.ManipulatedPlayer, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover()).ToList());
                else if (PlayerControl.LocalPlayer.IsLover() && !CustomGameOptions.MadmateKillEachOther) Utils.SetTargetPlayer(ref target, killButton, role.ManipulatedPlayer, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Impostors) && !x.Is(Faction.Madmates)).ToList());
                else if (PlayerControl.LocalPlayer.IsLover()) Utils.SetTargetPlayer(ref target, killButton, role.ManipulatedPlayer, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Impostors)).ToList());
                else if (!CustomGameOptions.MadmateKillEachOther) Utils.SetTargetPlayer(ref target, killButton, role.ManipulatedPlayer, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Impostors) && !x.Is(Faction.Madmates)).ToList());
                else Utils.SetTargetPlayer(ref target, killButton, role.ManipulatedPlayer, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Impostors)).ToList());
                killButton.SetTarget(target);

                return;
            }

            if (!killButton.isActiveAndEnabled) target = null;
            else if (!PlayerControl.LocalPlayer.moveable) target = null;
            else if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref target, killButton);
            else if (PlayerControl.LocalPlayer.IsLover() && CustomGameOptions.ImpLoverKillTeammate) Utils.SetTarget(ref target, killButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover()).ToList());
            else if (PlayerControl.LocalPlayer.IsLover() && !CustomGameOptions.MadmateKillEachOther) Utils.SetTarget(ref target, killButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Impostors) && !x.Is(Faction.Madmates)).ToList());
            else if (PlayerControl.LocalPlayer.IsLover()) Utils.SetTarget(ref target, killButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Impostors)).ToList());
            else if (!CustomGameOptions.MadmateKillEachOther) Utils.SetTarget(ref target, killButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Impostors) && !x.Is(Faction.Madmates)).ToList());
            else Utils.SetTarget(ref target, killButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Impostors)).ToList());
            killButton.SetTarget(target);
        }
    }
}
