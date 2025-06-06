using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.neutralRoles.InfectiousMod
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
    public static class PlayerPhysics_FixedUpdate
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            foreach (var infectious in Role.GetRoles(RoleEnum.Infectious))
            {
                var infectiousRole = (Infectious)infectious;
                if (!infectiousRole.Infected.Contains(__instance.myPlayer.PlayerId)
                || infectiousRole.Player.Data.IsDead) return;
                if (__instance.myPlayer.Data.IsDead) return;
                var role = Role.GetRole(__instance.myPlayer);
                if (role == null) return;
                if (role.InfectionState == 0) return;
                if (__instance.myPlayer.Is(RoleEnum.Infectious)) return;
                if (role.InfectionState > 1)
                {
                    __instance.body.velocity *= CustomGameOptions.InfectedSpeed;
                }
            }
        }
    }
}