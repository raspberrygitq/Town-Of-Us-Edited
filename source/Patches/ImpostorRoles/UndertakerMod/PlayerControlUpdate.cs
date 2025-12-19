using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.ImpostorRoles.UndertakerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class PlayerControlUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Undertaker)) return;

            var role = Role.GetRole<Undertaker>(PlayerControl.LocalPlayer);
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

            role.DragDropButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            role.DragDropButton.buttonLabelText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.DragDropButton.transform.localPosition = new Vector3(-2f, 1f, 0f);

            role.DragDropButton.buttonLabelText.text = role.DragDropButton.graphic.sprite == TownOfUsEdited.DropSprite ? "Drop" : "Drag";
            
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
                var killButton = role.DragDropButton;
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


                KillButtonTarget.SetTarget(killButton, closestBody, role);
            }

            if (role.DragDropButton.graphic.sprite == TownOfUsEdited.DragSprite)
            {
                role.DragDropButton.SetCoolDown(role.DragTimer(), CustomGameOptions.DragCd);
            }
            else
            {
                role.DragDropButton.SetCoolDown(0f, 1f);
                role.DragDropButton.graphic.color = Palette.EnabledColor;
                role.DragDropButton.graphic.material.SetFloat("_Desat", 0f);
                role.DragDropButton.buttonLabelText.color = Palette.EnabledColor;
                role.DragDropButton.buttonLabelText.material.SetFloat("_Desat", 0f);
            }

            role.DragDropButton.graphic.SetCooldownNormalizedUvs();
        }
    }
}