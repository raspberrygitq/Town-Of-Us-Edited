using System.Collections;
using HarmonyLib;
using Reactor.Utilities;
using TownOfUsEdited.Extensions;
using UnityEngine;

namespace TownOfUsEdited.Patches {
    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    static class LobbyBehaviourPatch {
        [HarmonyPostfix]
        public static void Postfix() {
            // Fix Killed When Zooming As Captain
            FixScreen.UnZoomFix();
            // Clear upped Players list
            RpcHandling.Upped.Clear();
            // Fix Grenadier blind in lobby
            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = false;
            // Auto Start code
            if (AmongUsClient.Instance.AmHost && CustomGameOptions.AutoStart)
            {
                HostManager.starting = false;
                Coroutines.Start(HostManager.AutoStart());
            }
            // Welcome message
            ResetChatSent.welcomesent = false;
        }
    }
    public class FixScreen
    {
        public static void UnZoomFix()
        {
            var size = 3f;
            Camera.main.orthographicSize = size;

            foreach (var cam in Camera.allCameras)
            {
                if (cam?.gameObject.name == "UI Camera")
                    cam.orthographicSize = size;
            }

            ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);
        }
    }
}
