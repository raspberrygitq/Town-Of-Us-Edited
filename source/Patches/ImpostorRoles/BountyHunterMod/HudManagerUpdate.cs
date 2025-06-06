using System;
using System.Collections.Generic;
using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUsEdited.ImpostorRoles.BountyHunterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite Arrow => TownOfUsEdited.Arrow;
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.BountyHunter)) return;

            var role = Role.GetRole<BountyHunter>(PlayerControl.LocalPlayer);
            if (role.TimerButton == null)
            {
                role.TimerButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.TimerButton.graphic.enabled = true;
                role.TimerButton.gameObject.SetActive(false);
                role.TimerText = Object.Instantiate(__instance.KillButton.buttonLabelText, role.TimerButton.transform);
                role.TimerText.gameObject.SetActive(false);
                role.ButtonLabels.Add(role.TimerText);
            }

            role.TimerButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            role.TimerButton.graphic.sprite = TownOfUsEdited.Bounty;

            role.TimerButton.transform.localPosition = new Vector3(-2f, 1f, 0f);

            role.TimerText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.TimerText.text = "Timer";
            role.TimerText.SetOutlineColor(Palette.ImpostorRed);

            var renderer = role.TimerButton.graphic;
            renderer.color = Palette.EnabledColor;
            renderer.material.SetFloat("_Desat", 0f);
            role.TimerText.color = Palette.EnabledColor;
            role.TimerText.material.SetFloat("_Desat", 0f);
            role.TimerButton.SetCoolDown(role.TargetTimer(), CustomGameOptions.TargetDuration);

            if ((role.TargetTimer() <= 0f || role.BountyTarget == null || role.BountyTarget.Data.IsDead || role.BountyTarget.Data.Disconnected) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var bhTargets = new List<PlayerControl>();
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (!player.Is(Faction.Impostors) && !player.Is(Faction.Madmates) && !player.Data.IsDead && !player.Data.Disconnected && !(player.IsLover() && role.Player.IsLover()))
                    {
                        bhTargets.Add(player);
                    }
                }
                foreach (var Arrow in role.TargetArrow.Keys)
                {
                    role.DestroyArrow(Arrow);
                    role.TargetArrow.Clear();
                }
                if (bhTargets.Count > 0)
                {
                    if (role.BountyTarget != null)
                    {
                        //So Bounty Hunter doesn't get the same target twice in a row
                        bhTargets.Remove(role.BountyTarget);
                    }
                    role.BountyTarget = bhTargets[UnityEngine.Random.RandomRangeInt(0, bhTargets.Count)];
                    role.TargetSwitch = DateTime.UtcNow;
                    role.TaskText = () => $"Kill your target to get a short kill cooldown\nCurrent Target: {role.BountyTarget.name}\nFake Tasks:";
                    role.RegenTask();
                    if (!role.TargetArrow.ContainsKey(role.BountyTarget.PlayerId))
                    {
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer2 = gameObj.AddComponent<SpriteRenderer>();
                        renderer2.sprite = Arrow;
                        arrow.image = renderer2;
                        gameObj.layer = 5;
                        role.TargetArrow.Add(role.BountyTarget.PlayerId, arrow);
                    }
                }
            }
        }
    }
}