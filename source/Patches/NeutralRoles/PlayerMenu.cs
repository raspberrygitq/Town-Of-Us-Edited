﻿using AmongUs.GameOptions;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.Patches.NeutralRoles
{
    public class PlayerMenu
    {
        public ShapeshifterMinigame Menu;
        public Select Click;
        public Include Inclusion;
        public List<PlayerControl> Targets;
        public static PlayerMenu singleton;
        public delegate void Select(PlayerControl player);
        public delegate bool Include(PlayerControl player);

        public PlayerMenu(Select click, Include inclusion)
        {
            Click = click;
            Inclusion = inclusion;
            if (singleton != null)
            {
                singleton.Menu.DestroyImmediate();
                singleton = null;
            }
            singleton = this;
        }

        public IEnumerator Open(float delay, bool includeDead = false)
        {
            yield return new WaitForSecondsRealtime(delay);
            while (ExileController.Instance != null) { yield return 0; }
            Targets = PlayerControl.AllPlayerControls.ToArray().Where(x => Inclusion(x) && (!x.Data.IsDead || includeDead) && !x.Data.Disconnected).ToList();
            Reactor.Utilities.Logger<TownOfUsEdited>.Warning($"Targets {Targets.Count}");
            if (Menu == null)
            {
                if (Camera.main == null)
                    yield break;

                Menu = GameObject.Instantiate(GetShapeshifterMenu(), Camera.main.transform, false);
            }

            Menu.transform.SetParent(Camera.main.transform, false);
            Menu.transform.localPosition = new(0f, 0f, -50f);
            Menu.Begin(null);
        }

        private static ShapeshifterMinigame GetShapeshifterMenu()
        {
            var rolePrefab = RoleManager.Instance.AllRoles.First(r => r.Role == RoleTypes.Shapeshifter);
            return GameObject.Instantiate(rolePrefab?.Cast<ShapeshifterRole>(), GameData.Instance.transform).ShapeshifterMenu;
        }

        public void Clicked(PlayerControl player)
        {
            Click(player);
            Menu.Close();
        }

        [HarmonyPatch(typeof(ShapeshifterMinigame), nameof(ShapeshifterMinigame.OnDisable))]
        public static class ClosePatch
        {
            public static void Postfix()
            {
                if (singleton == null) return;
                singleton.Menu.DestroyImmediate();
                singleton = null;
            }
        }

        [HarmonyPatch(typeof(ShapeshifterMinigame), nameof(ShapeshifterMinigame.Begin))]
        public static class MenuPatch
        {
            public static bool Prefix(ShapeshifterMinigame __instance)
            {
                var menu = PlayerMenu.singleton;

                if (menu == null || PlayerControl.LocalPlayer.Is(RoleEnum.Traitor))
                    return true;

                __instance.potentialVictims = new();
                var list2 = new Il2CppSystem.Collections.Generic.List<UiElement>();

                for (var i = 0; i < menu.Targets.Count; i++)
                {
                    var player = menu.Targets[i];
                    bool isDead = player.Data.IsDead;
                    player.Data.IsDead = false;
                    var num = i % 3;
                    var num2 = i / 3;
                    var panel = GameObject.Instantiate(__instance.PanelPrefab, __instance.transform);
                    panel.transform.localPosition = new(__instance.XStart + (num * __instance.XOffset), __instance.YStart + (num2 * __instance.YOffset), -1f);
                    panel.SetPlayer(i, player.Data, (Action)(() => menu.Clicked(player)));
                    __instance.potentialVictims.Add(panel);
                    list2.Add(panel.Button);
                    player.Data.IsDead = isDead;
                }

                ControllerManager.Instance.OpenOverlayMenu(__instance.name, __instance.BackButton, __instance.DefaultButtonSelected, list2);
                return false;
            }
        }
        [HarmonyPatch(typeof(ShapeshifterPanel), nameof(ShapeshifterPanel.SetPlayer))]
        public static class DoppelgangerPatch
        {
            public static void Postfix(ShapeshifterPanel __instance, [HarmonyArgument(1)] NetworkedPlayerInfo playerInfo)
            {
                var doppels = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Doppelganger) && !x.Data.IsDead && !x.Data.Disconnected && Role.GetRole<Doppelganger>(x).TransformedPlayer != null);
                if (doppels == null) return;
                foreach (var doppel in doppels)
                {
                    if (doppel.Data == playerInfo)
                    {
                        var role = Role.GetRole<Doppelganger>(doppel);
                        __instance.PlayerIcon.UpdateFromEitherPlayerDataOrCache(role.TransformedPlayer.Data, PlayerOutfitType.Default, PlayerMaterial.MaskType.ComplexUI, false, null);
                        __instance.LevelNumberText.text = ProgressionManager.FormatVisualLevel(role.TransformedPlayer.Data.PlayerLevel);
                        __instance.Background.sprite = ShipStatus.Instance.CosmeticsCache.GetNameplate(role.TransformedPlayer.Data.DefaultOutfit.NamePlateId).Image;
                        __instance.NameText.text = role.TransformedPlayer.Data.PlayerName;
                    }
                }
            }
        }
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
        public static class StartMeeting
        {
            public static void Prefix(PlayerControl __instance)
            {
                if (__instance == null) return;
                try
                {
                    PlayerMenu.singleton.Menu.Close();
                }
                catch { }
            }
        }
    }
}
