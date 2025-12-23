using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.NeutralRoles.MaulMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Maul);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Maul>(PlayerControl.LocalPlayer);
            if (role.Player.inVent) return false;

            if (__instance == role.RampageButton)
            {
                if (role.RampageCooldown > 0) return false;
                if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return false;
                var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                if (!abilityUsed) return false;

                role.TimeRemaining = CustomGameOptions.RampageDuration;
                role.Rampage();
                return false;
            }

            if (role.Cooldown > 0) return false;
            if (!role.Rampaged) return false;
            if (__instance != HudManager.Instance.KillButton) return true;
            if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return false;
            if (role.ClosestPlayer == null) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        LegacyGameOptions.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag3) return false;
            if (PlayerControl.LocalPlayer.IsControlled() && role.ClosestPlayer.Is(Faction.Coven))
            {
                Utils.Interact(role.ClosestPlayer, PlayerControl.LocalPlayer, true);
                return false;
            }

            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
            if (interact[4] == true) return false;
            else if (interact[0] == true)
            {
                role.Cooldown = CustomGameOptions.RampageCd;
                return false;
            }
            else if (interact[1] == true)
            {
                role.Cooldown = CustomGameOptions.TempSaveCdReset;
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}
