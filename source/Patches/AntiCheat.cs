using HarmonyLib;
using Reactor.Networking.Extensions;
using TownOfUsEdited.Extensions;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class AntiCheat
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.Collider.enabled == false && !PlayerControl.LocalPlayer.Data.IsDead
            && AmongUsClient.Instance.NetworkMode != NetworkModes.LocalGame && !PlayerControl.LocalPlayer.IsDev())
            {
                AmongUsClient.Instance.DisconnectWithReason("You were kicked for cheating, please stop.");
            }
        }
    }
}