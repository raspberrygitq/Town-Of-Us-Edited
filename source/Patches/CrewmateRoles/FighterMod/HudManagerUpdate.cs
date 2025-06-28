using System.Linq;
using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.FighterMod
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
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Fighter)) return;

            // Get the Fighter role instance
            var role = Role.GetRole<Fighter>(PlayerControl.LocalPlayer);

            // Check if the game state allows the KillButton to be active
            bool isKillButtonActive = __instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled;
            isKillButtonActive = isKillButtonActive && !MeetingHud.Instance && !player.Data.IsDead;
            isKillButtonActive = isKillButtonActive && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
            AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay);

            // Set KillButton's visibility
            __instance.KillButton.gameObject.SetActive(isKillButtonActive);

            // Set KillButton's cooldown
            __instance.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.FighterKCD);
            if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
            else if (role.Player.IsLover() && role.Player.Is(Faction.Madmates) && (!CustomGameOptions.MadmateKillEachOther || CustomGameOptions.GameMode == GameMode.Cultist)) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Impostors)).ToList());
            else if (role.Player.Is(Faction.Madmates) && (!CustomGameOptions.MadmateKillEachOther || CustomGameOptions.GameMode == GameMode.Cultist)) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Impostors)).ToList());
            else if (role.Player.IsLover()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover()).ToList());
            else Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);

            return;
            
        }
    }
}