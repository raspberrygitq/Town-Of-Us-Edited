using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.ImpostorRoles.VenererMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Venerer);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Venerer>(PlayerControl.LocalPlayer);
            if (__instance == role.AbilityButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.Cooldown > 0 || role.Kills < 1) return false;
                var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                if (!abilityUsed) return false;

                Utils.Rpc(CustomRPC.Camouflage, PlayerControl.LocalPlayer.PlayerId);
                role.TimeRemaining = CustomGameOptions.AbilityDuration;
                role.Ability();
                return false;
            }

            return true;
        }
    }
}