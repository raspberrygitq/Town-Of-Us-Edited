using HarmonyLib;
using TownOfUsEdited;

namespace ChaosMode
{
    public class ChaosModePatch
    {
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class EnableChat
        {
            public static void Postfix(HudManager __instance)
            {
                if (CustomGameOptions.GameMode == GameMode.Chaos && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started
                && !__instance.Chat.isActiveAndEnabled)
                    __instance.Chat.SetVisible(true);
            }
        }
    }
}