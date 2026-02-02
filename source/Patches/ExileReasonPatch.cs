using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
    internal class CustomExileController
    {
        private static void Postfix(ExileController __instance)
        {
            var exiled = __instance.initData?.networkedPlayer;
            if (exiled == null)
                return;

            var player = exiled.Object;

            var role = Role.GetRole(player);
            if (role == null)
                return;

            role.DeathReason = DeathReasons.Exiled;
        }
    }
}
