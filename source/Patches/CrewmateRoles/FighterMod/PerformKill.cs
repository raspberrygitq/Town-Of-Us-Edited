using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.Patches.CrewmateRoles.FighterMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Fighter))
                return true;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            var role = Role.GetRole<Fighter>(PlayerControl.LocalPlayer);
            
            if (role.ClosestPlayer == null)
                return false;
            
                if (role.Cooldown > 0)
                return false;

                if (__instance.isCoolingDown)
                return false;

                if (!__instance.isActiveAndEnabled)
                return false;

                if (PlayerControl.LocalPlayer.IsJailed()) return false;

                if (PlayerControl.LocalPlayer.IsControlled() && role.ClosestPlayer.Is(Faction.Coven))
                {
                    Utils.Interact(role.ClosestPlayer, PlayerControl.LocalPlayer, true);
                    return false;
                }
                else if (role.ClosestPlayer.Is(RoleEnum.PotionMaster) && Role.GetRole<PotionMaster>(role.ClosestPlayer).UsingPotion
                && Role.GetRole<PotionMaster>(role.ClosestPlayer).Potion == "Shield")
                {
                    role.Cooldown = CustomGameOptions.PotionKCDReset;
                    return false;
                }

                // Kill the closest player
                role.Kill(role.ClosestPlayer);

            return false;
        }
    }
}