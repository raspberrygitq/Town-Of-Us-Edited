using System.Linq;
using HarmonyLib;
using TownOfUsEdited.Patches;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CrewmateRoles.PlumberMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class VentButtonSprite
    {
        public static Sprite Flush = TownOfUsEdited.FlushSprite;

        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Plumber)) return;

            var role = Role.GetRole<Plumber>(PlayerControl.LocalPlayer);

            var blockButton = __instance.KillButton;

            if (role.FlushButton == null)
            {
                role.FlushButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.FlushButton.graphic.enabled = true;
                role.FlushButton.gameObject.SetActive(false);
            }

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(blockButton.cooldownTimerText, blockButton.transform);
                role.UsesText.gameObject.SetActive(false);
                role.UsesText.transform.localPosition = new Vector3(
                    role.UsesText.transform.localPosition.x + 0.26f,
                    role.UsesText.transform.localPosition.y + 0.29f,
                    role.UsesText.transform.localPosition.z);
                role.UsesText.transform.localScale = role.UsesText.transform.localScale * 0.65f;
                role.UsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.UsesText.fontStyle = TMPro.FontStyles.Bold;
            }
            if (role.UsesText != null)
            {
                role.UsesText.text = role.UsesLeft + "";
            }

            role.FlushButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            blockButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            role.UsesText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            if (role.FlushButton.isActiveAndEnabled)
            {
                role.FlushButton.transform.localPosition = new Vector3(-2f, 0f, 0f);
                role.FlushButton.graphic.sprite = Flush;
                role.FlushButton.buttonLabelText.text = "";
                role.FlushButton.SetCoolDown(role.FlushTimer(), CustomGameOptions.FlushCd);
                role.FlushButton.graphic.SetCooldownNormalizedUvs();
            }

            blockButton.SetTarget(null);
            blockButton.SetCoolDown(role.Cooldown, CustomGameOptions.FlushCd);
            var flushRenderer = role.FlushButton.graphic;
            var renderer = blockButton.graphic;

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var maxDistance = 0.75f; // Max distance from vent
            var flag = (GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks || !data.IsDead) &&
                       (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                       PlayerControl.LocalPlayer.CanMove;
            var AllVents = ShipStatus.Instance.AllVents;

            Vent closestVent = null;

            foreach (var vent in AllVents)
            {
                var flag2 = true;
                foreach (var role2 in Role.AllRoles.Where(x => x.RoleType == RoleEnum.Plumber))
                {
                    var plumber = (Plumber)role2;
                    if (plumber.VentsBlocked.Contains((byte)vent.Id))
                    {
                        flag2 = false;
                    }
                }
                if (!flag || isDead || !flag2)
                {
                    vent.myRend.material.SetFloat("_Outline", 0f);
                    vent.myRend.material.SetColor("_OutlineColor", Color.clear);
                    vent.myRend.material.SetColor("_AddColor", Color.clear);
                    continue;
                }
                if (SubmergedCompatibility.isSubmerged() && (vent.Id == 0 || vent.Id == 14)) continue;
                Vector3 center = PlayerControl.LocalPlayer.Collider.bounds.center;
                Vector3 position = vent.transform.position;
                if (Vector2.Distance((Vector2)center, (Vector2)position) <=
                3.5f)
                {
                    vent.myRend.material.SetFloat("_Outline", 1f);
                    vent.myRend.material.SetColor("_OutlineColor", role.Color);
                }
                else
                {
                    vent.myRend.material.SetFloat("_Outline", 0f);
                    vent.myRend.material.SetColor("_OutlineColor", Color.clear);
                }
                if (Vector2.Distance((Vector2)center, (Vector2)position) <=
                maxDistance)
                {
                    closestVent = vent;
                    vent.myRend.material.SetColor("_AddColor", role.Color);
                }
                else vent.myRend.material.SetColor("_AddColor", Color.clear);
            }

            role.Vent = closestVent;

            if (role.Vent != null && blockButton.enabled && PlayerControl.LocalPlayer.moveable)
            {
                flushRenderer.color = Palette.EnabledColor;
                flushRenderer.material.SetFloat("_Desat", 0f);
                if (role.ButtonUsable && !role.FutureBlocks.Contains((byte)role.Vent.Id))
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                    role.UsesText.color = Palette.EnabledColor;
                    role.UsesText.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    renderer.color = Palette.DisabledClear;
                    renderer.material.SetFloat("_Desat", 1f);
                    role.UsesText.color = Palette.DisabledClear;
                    role.UsesText.material.SetFloat("_Desat", 1f);
                }
            }
            else
            {
                flushRenderer.color = Palette.DisabledClear;
                flushRenderer.material.SetFloat("_Desat", 1f);
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
                role.UsesText.color = Palette.DisabledClear;
                role.UsesText.material.SetFloat("_Desat", 1f);
            }
        }
    }
}