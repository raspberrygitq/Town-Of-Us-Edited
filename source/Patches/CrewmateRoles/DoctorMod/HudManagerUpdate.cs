using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CrewmateRoles.DoctorMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudManagerUpdate
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateDragButton(__instance);
            UpdateReviveButton(__instance);
        }
        public static void UpdateReviveButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Doctor)) return;

            var role = Role.GetRole<Doctor>(PlayerControl.LocalPlayer);

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = LegacyGameOptions.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            var flag = (GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks || !data.IsDead) &&
                       (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                       PlayerControl.LocalPlayer.CanMove;
            var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance,
                LayerMask.GetMask(new[] { "Players", "Ghost" }));

            var reviveButton = __instance.KillButton;
            DeadBody closestBody = null;
            var closestDistance = float.MaxValue;

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(reviveButton.cooldownTimerText, reviveButton.transform);
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

            foreach (var collider2D in allocs)
            {
                if (!flag || isDead || collider2D.tag != "DeadBody") continue;
                var component = collider2D.GetComponent<DeadBody>();


                if (!(Vector2.Distance(truePosition, component.TruePosition) <=
                      maxDistance)) continue;

                var distance = Vector2.Distance(truePosition, component.TruePosition);
                if (!(distance < closestDistance)) continue;
                closestBody = component;
                closestDistance = distance;
            }

            reviveButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            role.UsesText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay)
                    && CustomGameOptions.GameMode != GameMode.Chaos);
            reviveButton.buttonLabelText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            reviveButton.graphic.sprite = TownOfUsEdited.DocReviveButton;

            reviveButton.buttonLabelText.text = "Revive";
            reviveButton.buttonLabelText.SetOutlineColor(Patches.Colors.Doctor);

            if (role.ButtonUsable || CustomGameOptions.GameMode == GameMode.Chaos)
            {
                reviveButton.SetCoolDown(role.ReviveTimer(), CustomGameOptions.DocReviveCooldown);
            }
            else
            {
                reviveButton.SetCoolDown(0f, CustomGameOptions.DocReviveCooldown);
            }

            foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
            {
                var player = Utils.PlayerById(deadBody.ParentId);
                var playerRole = Role.GetRole(player);
                if (closestBody != null && playerRole.InfectionState == 4 && closestBody.ParentId == deadBody.ParentId)
                {
                    if (!reviveButton.isCoolingDown)
                    {
                        var renderer = reviveButton.graphic;
                        renderer.color = Palette.DisabledClear;
                        renderer.material.SetFloat("_Desat", 1f);
                        role.UsesText.color = Palette.DisabledClear;
                        role.UsesText.material.SetFloat("_Desat", 1f);
                    }
                    return;
                }
            }

            if (role.ButtonUsable || CustomGameOptions.GameMode == GameMode.Chaos)
            {
                if (role.CanRevive() == true || CustomGameOptions.GameMode == GameMode.Chaos)
                    KillButtonTarget.SetTarget(reviveButton, closestBody, role);
                else
                    KillButtonTarget.SetTarget(reviveButton, null, role);
                KillButtonTarget.SetTarget(role._dragDropButton, closestBody, role);
            }
            else
            {
                reviveButton.SetCoolDown(0f, CustomGameOptions.DocReviveCooldown);
                var renderer = reviveButton.graphic;
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
                role.UsesText.color = Palette.DisabledClear;
                role.UsesText.material.SetFloat("_Desat", 1f);
            }
        }
        public static void UpdateDragButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Doctor)) return;

            var role = Role.GetRole<Doctor>(PlayerControl.LocalPlayer);
            if (role.DragDropButton == null)
            {
                role.DragDropButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.DragDropButton.graphic.enabled = true;
                role.DragDropButton.graphic.sprite = TownOfUsEdited.DragSprite;
                role.DragDropButton.gameObject.SetActive(false);
            }
            if (role.DragDropButton.graphic.sprite != TownOfUsEdited.DragSprite &&
                role.DragDropButton.graphic.sprite != TownOfUsEdited.DropSprite)
                role.DragDropButton.graphic.sprite = TownOfUsEdited.DragSprite;

            if (role.DragDropButton.graphic.sprite == TownOfUsEdited.DropSprite && role.CurrentlyDragging == null)
                role.DragDropButton.graphic.sprite = TownOfUsEdited.DragSprite;

            role.DragDropButton.transform.localPosition = new Vector3(-1f, 1f, 0f);
            role.DragDropButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay)
                    && CustomGameOptions.OnlyMedRevive && CustomGameOptions.GameMode != GameMode.Chaos);
            role.DragDropButton.buttonLabelText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay)
                    && CustomGameOptions.OnlyMedRevive && CustomGameOptions.GameMode != GameMode.Chaos);

            role.DragDropButton.transform.localPosition = new Vector3(-2f, 0f, 0f);

            role.DragDropButton.buttonLabelText.text = role.DragDropButton.graphic.sprite == TownOfUsEdited.DropSprite ? "Drop" : "Drag";
            role.DragDropButton.buttonLabelText.SetOutlineColor(Patches.Colors.Doctor);

            if (role.DragDropButton.graphic.sprite == TownOfUsEdited.DragSprite)
            {
                var data = PlayerControl.LocalPlayer.Data;
                var isDead = data.IsDead;
                var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                var maxDistance = LegacyGameOptions.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
                var flag = (GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks || !data.IsDead) &&
                           (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                           PlayerControl.LocalPlayer.CanMove;
                var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance,
                    LayerMask.GetMask(new[] { "Players", "Ghost" }));
                var dragButton = role.DragDropButton;
                DeadBody closestBody = null;
                var closestDistance = float.MaxValue;

                foreach (var collider2D in allocs)
                {
                    if (!flag || isDead || collider2D.tag != "DeadBody") continue;
                    var component = collider2D.GetComponent<DeadBody>();
                    if (!(Vector2.Distance(truePosition, component.TruePosition) <=
                          maxDistance)) continue;

                    var distance = Vector2.Distance(truePosition, component.TruePosition);
                    if (!(distance < closestDistance)) continue;
                    bool someoneDragging = false;
                    foreach (var diener in Role.GetRoles(RoleEnum.Undertaker))
                    {
                        if (diener.Player == PlayerControl.LocalPlayer) continue;
                        var dienerRole = (Undertaker)diener;
                        if (dienerRole.CurrentlyDragging == component)
                        {
                            someoneDragging = true;
                            continue;
                        }
                    }
                    foreach (var doctor in Role.GetRoles(RoleEnum.Doctor))
                    {
                        if (doctor.Player == PlayerControl.LocalPlayer) continue;
                        var doctorRole = (Doctor)doctor;
                        if (doctorRole.CurrentlyDragging == component)
                        {
                            someoneDragging = true;
                            continue;
                        }
                    }
                    if (someoneDragging) continue;
                    closestBody = component;
                    closestDistance = distance;
                }

                foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
                {
                    var player = Utils.PlayerById(deadBody.ParentId);
                    var playerRole = Role.GetRole(player);
                    if (closestBody != null && playerRole.InfectionState == 4 && closestBody.ParentId == deadBody.ParentId)
                    {
                        return;
                    }
                }
                KillButtonTarget.SetTarget(dragButton, closestBody, role);
            }
            role.DragDropButton.SetCoolDown(0f, 1f);
            role.DragDropButton.graphic.color = Palette.EnabledColor;
            role.DragDropButton.graphic.material.SetFloat("_Desat", 0f);
            role.DragDropButton.buttonLabelText.color = Palette.EnabledColor;
            role.DragDropButton.buttonLabelText.material.SetFloat("_Desat", 0f);
        }
    }
}