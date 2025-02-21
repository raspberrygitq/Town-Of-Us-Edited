using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CovenRoles.PotionMasterMod
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
    public static class PlayerPhysics_FixedUpdate
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.PotionMaster))
            {
                var pm = (PotionMaster)role;
                if (__instance.myPlayer == pm.Player && pm.UsingPotion && pm.Potion == "Speed")
                {
                    if (__instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove)
                    {
                        __instance.body.velocity = __instance.body.velocity * CustomGameOptions.PotionSpeed;
                    }
                }
            }
        }
    }
}