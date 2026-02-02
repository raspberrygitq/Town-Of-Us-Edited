using HarmonyLib;
using Reactor.Utilities;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUsEdited
{
    public class SabotageEffects
    {
        public static SpriteRenderer FullScreen;
        [HarmonyPatch(typeof(NoOxyTask), nameof(NoOxyTask.Initialize))]
        public class NoOxyEffect
        {
            public static void Postfix()
            {
                if (!CustomGameOptions.OxygenBlackout) return;
                int mapid;
                if (AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay) mapid = AmongUsClient.Instance.TutorialMapId;
                else mapid = GameOptionsManager.Instance.currentNormalGameOptions.MapId;

                if (mapid == 6) return; // Submerged already has a blackout screen effect for oxygen
                if (FullScreen == null) FullScreen = Object.Instantiate(HudManager.Instance.FullScreen, HudManager.Instance.FullScreen.transform.parent);
                var position = HudManager.Instance.transform.position;
                FullScreen.transform.position = new Vector3(position.x, position.y, -1f);
                if (mapid == 1) Coroutines.Start(FadeFullScreen(45f, Color.clear, Color.black));
                else Coroutines.Start(FadeFullScreen(30f, Color.clear, Color.black));
            }
        }
        [HarmonyPatch(typeof(ReactorTask), nameof(ReactorTask.Awake))]
        public class ReactorEffect
        {
            public static void Postfix()
            {
                if (!CustomGameOptions.ReactorScreenShake) return;
                int mapid;
                if (AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay) mapid = AmongUsClient.Instance.TutorialMapId;
                else mapid = GameOptionsManager.Instance.currentNormalGameOptions.MapId;

                if (mapid == 1 || mapid == 6) Coroutines.Start(ShakeScreen(45f, 0f, 1f));
                else if (mapid == 2 || mapid == 5) Coroutines.Start(ShakeScreen(60f, 0f, 1f));
                else Coroutines.Start(ShakeScreen(30f, 0f, 1f));
            }
        }

        [HarmonyPatch(typeof(HeliCharlesTask), nameof(HeliCharlesTask.Awake))]
        public class HeliEffect
        {
            public static void Postfix()
            {
                if (!CustomGameOptions.ReactorScreenShake) return;

                Coroutines.Start(ShakeScreen(90f, 0f, 1f));
            }
        }

        private static IEnumerator ShakeScreen(float duration, float startingSeverity, float maxSeverity)
        {
            var camera = Camera.main.gameObject.GetComponent<FollowerCamera>();
            ReactorSystemType reactor = null;
            ReactorSystemType seismic = null;
            HeliSabotageSystem heli = null;
            bool hasReactor = true;
            bool hasSeismic = true;
            bool hasHeli = true;
            try
            {
                reactor = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
            }
            catch
            {
                hasReactor = false;
            }
            try
            {
                seismic = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
            }
            catch
            {
                hasSeismic = false;
            }
            try
            {
                heli = ShipStatus.Instance.Systems[SystemTypes.HeliSabotage].Cast<HeliSabotageSystem>();
            }
            catch
            {
                hasHeli = false;
            }
            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                if ((hasReactor && reactor != null && !reactor.IsActive) || (hasSeismic && seismic != null && !seismic.IsActive) || (hasHeli && heli != null && !heli.IsActive) || (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.NetworkMode != NetworkModes.FreePlay))
                {
                    camera.Offset = Vector2.zero;
                    yield break;
                }
                camera.Offset = UnityEngine.Random.insideUnitCircle * Mathf.Lerp(startingSeverity, maxSeverity, t / duration);
                yield return null;
            }
            camera.Offset = Vector2.zero;
            yield break;
        }

        public static IEnumerator FadeFullScreen(float duration, Color startingColor, Color endingColor)
        {
            if (FullScreen.gameObject.activeSelf && FullScreen.color == endingColor)
            {
                yield break;
            }
            FullScreen.gameObject.SetActive(true);
            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                if (!FullScreen)
                {
                    yield break;
                }
                LifeSuppSystemType oxygen = null;
                bool hasOxy = true;
                try
                {
                    oxygen = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                }
                catch
                {
                    hasOxy = false;
                }
                if (!hasOxy || oxygen == null || !oxygen.IsActive || (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.NetworkMode != NetworkModes.FreePlay))
                {
                    FullScreen.gameObject.SetActive(false);
                    yield break;
                }
                FullScreen.color = Color.Lerp(startingColor, endingColor, t / duration);
                yield return null;
            }
            if (FullScreen)
            {
                FullScreen.color = endingColor;
                FullScreen.gameObject.SetActive(false);
            }
            yield break;
        }
    }
}