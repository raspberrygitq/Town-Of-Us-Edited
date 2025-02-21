
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor.Networking.Extensions;

namespace TownOfUsEdited.Patches
{
    public class BUE_2249530
    {
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class BUE_2249530_IST
        {

            public static void Postfix()
            {
                if (LobbyBehaviour.Instance || AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
                {
                    bool hasue = IL2CPPChainloader.Instance.Plugins.TryGetValue("com.sinai.unityexplorer", out PluginInfo plugin);
                    if (hasue)
                    {
                        AmongUsClient.Instance.DisconnectWithReason("You are not allowed to use UnityExplorer, please remove it and restart your game to play.");
                    }
                }
            }
        }
    }
}