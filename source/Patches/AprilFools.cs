using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Reactor.Utilities.Extensions;
using TMPro;
using TownOfUsEdited.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch]

    public class AprilFoolsPatches
    {
        public static bool IsAprilFools()
        {
            try
            {
                DateTime utcNow = DateTime.UtcNow;
                DateTime t = new DateTime(utcNow.Year, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                DateTime t2 = new DateTime(utcNow.Year, 4, 2, 0, 0, 0, 0, DateTimeKind.Utc);
                if (utcNow >= t && utcNow <= t2)
                {
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }
        public static int CurrentMode = 0;
        public static Dictionary<byte, GameObject> Hehs = new Dictionary<byte, GameObject>();
        public static float OriginalFlipX = 0f;

        public static Dictionary<int, string> Modes = new()
        {
            {0, "Off"},
            {1, "Horse"},
            {2, "Long"}
        };

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static void Postfix()
        {
            if (!IsAprilFools() || !PlayerControl.LocalPlayer || PlayerControl.AllPlayerControls.Count <= 0)
            {
                while (Hehs.Keys.Any())
                {
                    var heh1 = Hehs.FirstOrDefault();
                    if (heh1.Value != null) Object.Destroy(heh1.Value.gameObject);
                    Hehs.Remove(heh1.Key);
                }
                return;
            }
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.Disconnected)
                {
                    if (Hehs.TryGetValue(player.PlayerId, out GameObject hehObject2))
                    {
                        if (hehObject2 != null) Object.Destroy(hehObject2);
                        Hehs.Remove(player.PlayerId);
                    }
                    continue;
                }
                if (player == null || player.Data == null) continue;
                if (!Hehs.ContainsKey(player.PlayerId))
                {
                    var heh = new GameObject($"HEH{player.PlayerId}");
                    SpriteRenderer render = heh.AddComponent<SpriteRenderer>();
                    render.sprite = TownOfUsEdited.heh;
                    Hehs.Add(player.PlayerId, heh);
                    if (OriginalFlipX == 0f) OriginalFlipX = heh.transform.localScale.x;
                }
                Hehs.TryGetValue(player.PlayerId, out GameObject hehObject);
                var position = player.transform.localPosition;
                hehObject.transform.localPosition = new Vector3(position.x, position.y, -1f);
                var scale = hehObject.transform.localScale;
                bool flipped = player.cosmetics.currentBodySprite.BodySprite.flipX;
                if (flipped) hehObject.transform.localScale = new Vector3(OriginalFlipX * -1, scale.y, scale.z);
                else hehObject.transform.localScale = new Vector3(OriginalFlipX * 1, scale.y, scale.z);

                hehObject.SetActive(!player.Data.Disconnected && (!player.Data.IsDead || PlayerControl.LocalPlayer.Data.IsDead) &&
                player.nameText().color != Color.clear && !player.inVent);
            }
            foreach (var key in Hehs.Keys)
            {
                if (key > PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.Disconnected).ToList().Count - 1)
                {
                    Hehs.TryGetValue(key, out GameObject hehObject);
                    if (hehObject != null) Object.Destroy(hehObject);
                    Hehs.Remove(key);
                }
            }
        }

        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
        [HarmonyPrefix]

        public static void Prefix(MainMenuManager __instance)
        {
            if (__instance.newsButton != null)
            {

                var aprilfoolstoggle = UnityEngine.Object.Instantiate(__instance.newsButton, null);
                aprilfoolstoggle.name = "aprilfoolstoggle";

                aprilfoolstoggle.transform.localScale = new Vector3(0.44f, 0.84f, 1f);

                PassiveButton passive = aprilfoolstoggle.GetComponent<PassiveButton>();
                passive.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();

                aprilfoolstoggle.gameObject.transform.SetParent(GameObject.Find("RightPanel").transform);
                var pos = aprilfoolstoggle.gameObject.AddComponent<AspectPosition>();
                pos.Alignment = AspectPosition.EdgeAlignments.LeftBottom;
                pos.DistanceFromEdge = new Vector3(2.1f, 2f, 8f);

                passive.OnClick.AddListener((Action)(() =>
                {
                    int num = CurrentMode + 1;
                    CurrentMode = num > 2 ? 0 : num;
                    var text = aprilfoolstoggle.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>();
                    text.text = $"April fools mode: {Modes[CurrentMode]}";
                }));

                var text = aprilfoolstoggle.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>();
                __instance.StartCoroutine(Effects.Lerp(0.1f, new System.Action<float>((p) =>
                {
                    text.text = $"April fools mode: {Modes[CurrentMode]}";
                    pos.AdjustPosition();
                })));

                aprilfoolstoggle.transform.GetChild(0).transform.localScale = new Vector3(aprilfoolstoggle.transform.localScale.x + 1, 1f, 1f);
                aprilfoolstoggle.transform.GetChild(0).transform.localPosition -= new Vector3(1.5f,0f,0f);
                aprilfoolstoggle.transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
                aprilfoolstoggle.transform.GetChild(2).GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
                aprilfoolstoggle.GetComponent<NewsCountButton>().DestroyImmediate();
                aprilfoolstoggle.transform.GetChild(3).gameObject.DestroyImmediate();
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.SetBodyType))]
        [HarmonyPrefix]
        public static void Prefix(ref PlayerBodyTypes bodyType)
        {
            switch (CurrentMode)
            {
                case 1:
                    bodyType = PlayerBodyTypes.Horse;
                    break;
                case 2:
                    bodyType = PlayerBodyTypes.Long;
                    break;
            }
        }

        public class LongModePatches
        {
            [HarmonyPatch(typeof(LongBoiPlayerBody), nameof(LongBoiPlayerBody.Awake))]
            [HarmonyPrefix]
            public static bool Prefix(LongBoiPlayerBody __instance)
            {
                if (CurrentMode != 2) return false;
                __instance.cosmeticLayer.OnSetBodyAsGhost += DelegateSupport.ConvertDelegate<Il2CppSystem.Action>(__instance.SetPoolableGhost);
                __instance.cosmeticLayer.OnColorChange += DelegateSupport.ConvertDelegate<Il2CppSystem.Action<int>>(__instance.SetHeightFromColor);
                __instance.cosmeticLayer.OnCosmeticSet += DelegateSupport.ConvertDelegate<Il2CppSystem.Action<string, int, CosmeticsLayer.CosmeticKind>>(__instance.OnCosmeticSet);
                return false;
            }

            [HarmonyPatch(typeof(LongBoiPlayerBody), nameof(LongBoiPlayerBody.OnDestroy))]
            [HarmonyPrefix]
            public static bool Prefix2(LongBoiPlayerBody __instance)
            {
                if (CurrentMode != 2) return false;
                __instance.cosmeticLayer.OnSetBodyAsGhost -= DelegateSupport.ConvertDelegate<Il2CppSystem.Action>(__instance.SetPoolableGhost);
                __instance.cosmeticLayer.OnColorChange -= DelegateSupport.ConvertDelegate<Il2CppSystem.Action<int>>(__instance.SetHeightFromColor);
                __instance.cosmeticLayer.OnCosmeticSet -= DelegateSupport.ConvertDelegate<Il2CppSystem.Action<string, int, CosmeticsLayer.CosmeticKind>>(__instance.OnCosmeticSet);
                return false;
            }

            [HarmonyPatch(typeof(LongBoiPlayerBody), nameof(LongBoiPlayerBody.Start))]
            [HarmonyPrefix]
            public static bool Prefix3(LongBoiPlayerBody __instance)
            {
                if (CurrentMode != 2)
                {
                    __instance.ShouldLongAround = false;
                    __instance.headSprite.gameObject.SetActive(false);
                    __instance.neckSprite.gameObject.SetActive(false);
                    __instance.foregroundNeckSprite.gameObject.SetActive(false);
                    return false;
                }
                __instance.ShouldLongAround = true;
                if (__instance.hideCosmeticsQC)
                {
                    __instance.cosmeticLayer.SetHatVisorVisible(false);
                }
                __instance.SetupNeckGrowth(false, true);
                if (__instance.isExiledPlayer)
                {
                    ShipStatus instance = ShipStatus.Instance;
                    if (instance == null || instance.Type != ShipStatus.MapType.Fungle)
                    {
                        __instance.cosmeticLayer.AdjustCosmeticRotations(-17.75f);
                    }
                }
                if (!__instance.isPoolablePlayer)
                {
                    __instance.cosmeticLayer.ValidateCosmetics();
                }
                return false;
            }

            [HarmonyPatch(typeof(HatManager), nameof(HatManager.CheckLongModeValidCosmetic))]
            [HarmonyPrefix]

            public static bool Prefix4(HatManager __instance, [HarmonyArgument(0)] string cosmeticID, [HarmonyArgument(1)] bool ignoreLongMode, ref bool __result)
            {
                if (CurrentMode != 2)
                {
                    __result = true;
                    return false;
                }
                if (ignoreLongMode)
                {
                    __result = true;
                    return false;
                }
                foreach (var data in __instance.longModeBlackList)
                {
                    if (string.Equals(data.ProdId, cosmeticID))
                    {
                        __result = false;
                        return false;
                    }
                }
                __result = true;
                return false;
            }

            [HarmonyPatch(typeof(CosmeticsLayer), nameof(CosmeticsLayer.SetSkin))]
            [HarmonyPrefix]

            public static bool Prefix5(CosmeticsLayer __instance, [HarmonyArgument(0)] SkinData skin, [HarmonyArgument(1)] int color, [HarmonyArgument(2)] Action onLoaded)
            {
                if (CurrentMode != 2) return true;
                if (!__instance.skin)
                {
                    return false;
                }
                if (__instance.GetLongBoi() != null && !__instance.GetLongBoi().ValidateSkin(skin.ProdId, color))
                {
                    skin = DestroyableSingleton<HatManager>.Instance.GetSkinById("skin_None");
                }
                __instance.skin.SetSkin(skin, color, __instance.currentBodySprite.BodySprite.flipX, __instance, onLoaded);
                __instance.skin.Flipped = __instance.currentBodySprite.BodySprite.flipX;
                return false;
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.BodyType), MethodType.Getter)]
        [HarmonyPrefix]

        public static bool Prefix2(ref PlayerBodyTypes __result)
        {
            switch (CurrentMode)
            {
                case 1:
                    __result = PlayerBodyTypes.Horse;
                    return false;
                case 2:
                    __result = PlayerBodyTypes.Long;
                    return false;
                default:
                    return true;
            }
        }

        [HarmonyPatch(typeof(NormalGameManager), nameof(NormalGameManager.GetBodyType))]
        [HarmonyPrefix]

        public static bool Prefix3(ref PlayerBodyTypes __result)
        {
            switch (CurrentMode)
            {
                case 1:
                    __result = PlayerBodyTypes.Horse;
                    return false;
                case 2:
                    __result = PlayerBodyTypes.Long;
                    return false;
                default:
                    return true;
            }
        }
    }
}