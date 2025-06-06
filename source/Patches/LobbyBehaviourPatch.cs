using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Reactor.Utilities;
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
            HudManager.Instance.ShadowQuad.gameObject.SetActive(true);
        }
    }

    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Update))]
    public class LobbyBehaviourUpdate 
    {
        public static void Postfix(LobbyBehaviour __instance)
        {
            UpdateLobbyMusic(__instance);
        }
        public static void UpdateLobbyMusic(LobbyBehaviour __instance)
        {
            if (TownOfUsEdited.DisableLobbyMusic.Value && SoundManager.Instance.soundPlayers.ToArray().Any(x => x.Name == "MapTheme"))
            {
                SoundManager.Instance.StopSound(__instance.MapTheme);
                if (SoundManager.Instance.soundPlayers.ToArray().Any(x => x.Name == "MapTheme"))
                {
                    List<ISoundPlayer> toRemove = new List<ISoundPlayer>();
                    foreach (var audio in SoundManager.Instance.soundPlayers)
                    {
                        if (audio.Name == "MapTheme") toRemove.Add(audio);
                    }
                    foreach (var soundplayer in toRemove)
                    {
                        SoundManager.Instance.soundPlayers.Remove(soundplayer);
                    }
                }
                PluginSingleton<TownOfUsEdited>.Instance.Log.LogInfo("Stopped Lobby Music");
            }
            else if (!TownOfUsEdited.DisableLobbyMusic.Value && !SoundManager.Instance.soundPlayers.ToArray().Any(x => x.Name == "MapTheme"))
            {
                SoundManager.Instance.CrossFadeSound("MapTheme", __instance.MapTheme, 0.07f, 1.5f);
                PluginSingleton<TownOfUsEdited>.Instance.Log.LogInfo("Started Lobby Music");
            }
        }
    }
}
