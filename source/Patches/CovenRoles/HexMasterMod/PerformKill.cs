using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.Patches.CovenRoles.HexMasterMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.HexMaster))
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            var role = Role.GetRole<HexMaster>(PlayerControl.LocalPlayer);
            var killbutton = DestroyableSingleton<HudManager>.Instance.KillButton;
            
            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            if (__instance == killbutton)
            {
                if (role.ClosestPlayer == null)
                return false;
                
                if (role.Cooldown > 0)
                return false;

                // Hex the closest player
                role.Hex(role.ClosestPlayer);
            }

            else if (__instance == role.HexBombButton)
            {
                if (role.Cooldown > 0)
                return false;

                if (role.Hexed.Count <= 0) return false;

                role.HexBomb();
            }

            return false;
        }
    }
}