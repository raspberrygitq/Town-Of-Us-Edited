using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.Patches.CovenRoles.PotionMasterMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.PotionMaster))
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            var role = Role.GetRole<PotionMaster>(PlayerControl.LocalPlayer);
            
            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            if (__instance == role.PotionButton)
            {
                if (role.PotionCooldown > 0)
                return false;

                if (__instance.graphic.sprite == TownOfUs.Potion)
                {
                    role.GetPotion();
                    role.PotionCooldown = 5f;
                    __instance.graphic.sprite = TownOfUs.Drink;
                }
                else
                {
                    role.UsePotion();
                    role.TimeRemaining = CustomGameOptions.PotionDuration;
                    role.Enabled = true;
                    Utils.Rpc(CustomRPC.UsePotion, PlayerControl.LocalPlayer.PlayerId, role.Potion);
                }
            }

            return false;
        }
    }
}