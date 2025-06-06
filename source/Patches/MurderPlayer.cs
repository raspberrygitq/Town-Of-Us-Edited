using HarmonyLib;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch]
    public class MurderPlayer
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        public class MurderPlayerPatch
        {
            public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
            {
                if (LobbyBehaviour.Instance) return false;
                Utils.MurderPlayer(__instance, target, true);
                return false;
            }
        }
    }
}