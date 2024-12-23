using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.TrollMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Troll)) return;

            // Get the Troll role instance
            var troll = Role.GetRole<Troll>(PlayerControl.LocalPlayer);

            // Check if the game state allows the Button to be active
            bool isKillButtonActive = __instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled;
            isKillButtonActive = isKillButtonActive && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead;
            isKillButtonActive = isKillButtonActive && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started;
            isKillButtonActive = isKillButtonActive && troll.TrolledPlayer == null;

            // Set Button's graphics
            __instance.KillButton.buttonLabelText.text = "Troll";

            // Set Button's visibility
            __instance.KillButton.gameObject.SetActive(isKillButtonActive);

            // Set the closest player for the Button's targeting

            Utils.SetTarget(ref troll.ClosestPlayer, __instance.KillButton, float.NaN);

            return;
            
        }
    }
}