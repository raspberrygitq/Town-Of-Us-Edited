using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using Reactor.Utilities;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CrewmateRoles.PlumberMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static Sprite Arrow => TownOfUsEdited.Arrow;
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Plumber);
            if (!flag) return true;
            var role = Role.GetRole<Plumber>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!__instance.enabled || __instance.isCoolingDown) return false;
            if (role.Cooldown > 0f) return false;
            var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
            if (!abilityUsed) return false;
            if (__instance == role.FlushButton)
            {
                PluginSingleton<TownOfUsEdited>.Instance.Log.LogMessage($"{role.Vent.Id}");
                var someoneInVent = false;
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.inVent) someoneInVent = true;
                }
                if (someoneInVent)
                {
                    Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Plumber));
                    Coroutines.Start(SeeVenter());
                }
                role.Cooldown = CustomGameOptions.FlushCd;
                Utils.Rpc(CustomRPC.Flush, (byte)0);
                return false;
            }
            if (__instance != HudManager.Instance.KillButton) return true;
            if (role.Cooldown > 0f || role.Vent == null || !role.ButtonUsable) return false;
            if (!role.FutureBlocks.Contains((byte)role.Vent.Id) && !role.VentsBlocked.Contains((byte)role.Vent.Id)) role.FutureBlocks.Add((byte)role.Vent.Id);
            else return false;

            PluginSingleton<TownOfUsEdited>.Instance.Log.LogMessage($"{role.Vent.Id}");
            role.UsesLeft--;
            role.Cooldown = CustomGameOptions.FlushCd;
            Utils.Rpc(CustomRPC.Flush, (byte)1, role.Player.PlayerId, (byte)role.Vent.Id);
            return false;
        }

        public static IEnumerator SeeVenter()
        {
            var startTime = DateTime.UtcNow;
            var arrows = new Dictionary<byte, ArrowBehaviour>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.inVent)
                {
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = player.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Arrow;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    arrows.Add(player.PlayerId, arrow);
                }
            }

            while ((DateTime.UtcNow - startTime).TotalSeconds < 1f)
            {
                foreach (var arrow in arrows)
                {
                    arrows.GetValueSafe(arrow.Key).target = Utils.PlayerById(arrow.Key).transform.localPosition;
                }
                yield return null;
            }

            arrows.Values.DestroyAll();
            arrows.Clear();
        }
    }
}