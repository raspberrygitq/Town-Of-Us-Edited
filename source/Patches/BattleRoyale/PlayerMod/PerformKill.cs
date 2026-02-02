using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.Patches.BattleRoyale.PlayerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PlayerKillPatch
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Player))
                return true;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            var player = Role.GetRole<Player>(PlayerControl.LocalPlayer);
            var killbutton = HudManager.Instance.KillButton;

            if (player.ClosestPlayer == null)
                return false;

            if (__instance == killbutton)
            {
                if (player.Cooldown > 0)
                    return false;

                // Kill the closest player
                player.Kill(player.ClosestPlayer);
            }

            return false;
        }
    }
}