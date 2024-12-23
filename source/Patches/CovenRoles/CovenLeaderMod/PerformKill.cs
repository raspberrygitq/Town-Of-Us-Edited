using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.Patches.CovenRoles.CovenLeaderMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.CovenLeader))
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            var role = Role.GetRole<CovenLeader>(PlayerControl.LocalPlayer);
            var killbutton = DestroyableSingleton<HudManager>.Instance.KillButton;
                
            if (__instance == role.SabotageButton) 
            {
                DestroyableSingleton<HudManager>.Instance.ToggleMapVisible(new MapOptions
                {
                    Mode = MapOptions.Modes.Sabotage
                });
                return false;
            }
            
            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            if (__instance == killbutton)
            {
                if (PlayerControl.LocalPlayer.IsJailed()) return false;

                if (role.ClosestPlayer == null)
                return false;
                
                if (role.Cooldown > 0)
                return false;

                if (role.ClosestPlayer.IsGuarded2())
                {
                    role.Cooldown = CustomGameOptions.GuardKCReset;
                    return false; 
                }

                // Kill the closest player
                role.Kill(role.ClosestPlayer);
            }

            else if (__instance == role.RecruitButton)
            {
                if (PlayerControl.LocalPlayer.IsJailed()) return false;

                if (role.Cooldown > 0)
                return false;

                if (role.ClosestPlayer == null)
                return false;

                if (role.ClosestPlayer.IsGuarded2())
                {
                    role.Cooldown = CustomGameOptions.GuardKCReset;
                    return false; 
                }

                role.Recruit(role.ClosestPlayer);
            }

            return false;
        }
    }
}