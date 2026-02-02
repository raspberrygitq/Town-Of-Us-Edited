using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.Patches.CrewmateRoles.KnightMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Knight))
                return true;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            var knight = Role.GetRole<Knight>(PlayerControl.LocalPlayer);

            if (knight.ClosestPlayer == null)
                return false;

            if (!knight.ButtonUsable)
                return false;

            if (__instance.isCoolingDown)
                return false;

            if (!__instance.isActiveAndEnabled)
                return false;

            if (PlayerControl.LocalPlayer.IsControlled() && knight.ClosestPlayer.Is(Faction.Coven))
            {
                Utils.Interact(knight.ClosestPlayer, PlayerControl.LocalPlayer, true);
                return false;
            }

            // Kill the closest player
            knight.Kill(knight.ClosestPlayer);

            return false;
        }
    }
}