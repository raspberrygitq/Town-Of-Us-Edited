using System.Linq;
using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.BattleRoyale.PlayerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            var player = PlayerControl.LocalPlayer;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Player)) return;
            if (CustomGameOptions.GameMode != GameMode.BattleRoyale) return;

            // Get the Player role instance
            var playerRole = Role.GetRole<Player>(PlayerControl.LocalPlayer);

            // Check if the game state allows the KillButton to be active
            bool isKillButtonActive = __instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled;
            isKillButtonActive = isKillButtonActive && !MeetingHud.Instance && !player.Data.IsDead;
            isKillButtonActive = isKillButtonActive && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started;

            // Set KillButton's visibility
            __instance.KillButton.gameObject.SetActive(isKillButtonActive);

            // Set KillButton's cooldown
            __instance.KillButton.SetCoolDown(playerRole.KillTimer(), CustomGameOptions.BattleRoyaleKillCD);

            // Set the closest player for the Kill Button's targeting
            var notsk = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !x.Is(RoleEnum.Player) && !x.Data.IsDead)
                .ToList();

            Utils.SetTarget(ref playerRole.ClosestPlayer, __instance.KillButton, float.NaN);

            return;
            
        }
    }
}