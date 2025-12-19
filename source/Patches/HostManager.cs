using HarmonyLib;
using Il2CppSystem;
using Reactor.Utilities;
using System.Collections;
using UnityEngine;

namespace TownOfUsEdited.Patches
{
    public class HostManager
    {
        public static bool starting;
        public static IEnumerator AutoRejoin()
        {
            if (!AmongUsClient.Instance.AmHost) yield break;

            yield return new WaitForSeconds(CustomGameOptions.RejoinSeconds);

            if (LobbyBehaviour.Instance || AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started
            || AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.NotJoined) yield break;
            
            DestroyableSingleton<EndGameNavigation>.Instance.NextGame();

            yield break;
        }

        public static IEnumerator AutoStart()
        {
            if (!AmongUsClient.Instance.AmHost) yield break;
            if (!CustomGameOptions.AutoStart) yield break;

            var startTime = DateTime.UtcNow;
            while (true)
            {
                var now = DateTime.UtcNow;
                var seconds = (now - startTime).TotalSeconds;
                if (seconds < CustomGameOptions.StartMinutes * 60)
                    yield return null;
                else break;

                if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                !LobbyBehaviour.Instance)
                {
                    PluginSingleton<TownOfUsEdited>.Instance.Log.LogMessage("AutoStart Canceled");
                    yield break;
                }
            }

            if (LobbyBehaviour.Instance && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started
            && AmongUsClient.Instance.AmHost && !starting)
            {
                starting = true;
                Coroutines.Start(StartCountdown());
                PluginSingleton<TownOfUsEdited>.Instance.Log.LogMessage("AutoStart triggered");
            }

            yield break;
        }

        public static IEnumerator StartCountdown()
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                starting = false;
                yield break;
            }
            if (!LobbyBehaviour.Instance)
            {
                starting = false;
                yield break;
            }
            if (!CustomGameOptions.AutoStart)
            {
                starting = false;
                yield break;
            }
            GameStartManager.Instance.startState = GameStartManager.StartingStates.Countdown;
            GameStartManager.Instance.countDownTimer = 5f;
            yield return new WaitForSeconds(5);
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && LobbyBehaviour.Instance && starting == true)
            {
                Coroutines.Start(StartCountdown());
                yield break;
            }
            else
            {
                starting = false;
                yield break;
            }
        }

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
        public class FixHostMinPlayer
        {
            public static bool Prefix(GameStartManager __instance)
            {
                if (!AmongUsClient.Instance.AmHost) return false;
                if (__instance.startState != GameStartManager.StartingStates.NotStarting)
		        {
			        return false;
		        }
                __instance.ReallyBegin(false);
                return false;
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        [HarmonyPriority(Priority.Last)]
        public class PatchStartButtonColor
        {
            public static void Postfix()
            {
                if (!GameData.Instance)
		        {
			        return;
		        }
		        if (!GameManager.Instance)
		        {
			        return;
		        }
                if (!LobbyBehaviour.Instance) return;
                if (!AmongUsClient.Instance.AmHost) return;
                if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started) return;
                if (GameStartManager.Instance.startState != GameStartManager.StartingStates.NotStarting) return;
                if (GameStartManager.Instance.StartButton && GameStartManager.Instance.StartButtonGlyph.spriteRenderer.color == Palette.DisabledClear)
		        {
                    GameStartManager.Instance.StartButtonGlyph.spriteRenderer.color = Palette.EnabledColor;
                    GameStartManager.Instance.StartButton.SetButtonEnableState(true);
                    GameStartManager.Instance.StartButton.ChangeButtonText(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.StartLabel, string.Empty));
                }
            }
        }
    }
}