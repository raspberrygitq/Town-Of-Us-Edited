using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.Patches.NeutralRoles.DoppelgangerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Doppelganger))
                return true;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            var role = Role.GetRole<Doppelganger>(PlayerControl.LocalPlayer);
            var killbutton = HudManager.Instance.KillButton;
            
            if (role.ClosestPlayer == null)
                return false;
            
            if (__instance == killbutton)
            {
                if (role.Cooldown > 0)
                return false;

                if (PlayerControl.LocalPlayer.IsControlled() && role.ClosestPlayer.Is(Faction.Coven))
                {
                    Utils.Interact(role.ClosestPlayer, PlayerControl.LocalPlayer, true);
                    return false;
                }

                // Kill the closest player
                role.Kill(role.ClosestPlayer);
            }

            return false;
        }
    }
}