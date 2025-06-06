using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;
using TownOfUsEdited.Extensions;
using System.Linq;
using TMPro;

namespace TownOfUsEdited.Patches.CrewmateRoles.LookoutMod
{
    public class LookoutPatches
    {
        public static Transform TaskOverlay;
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class HudManagerUpdate
        {
            public static void Postfix(HudManager __instance)
            {
                var player = PlayerControl.LocalPlayer;
                if (PlayerControl.AllPlayerControls.Count <= 1) return;
                if (PlayerControl.LocalPlayer == null) return;
                if (PlayerControl.LocalPlayer.Data == null) return;
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Lookout)) return;

                var role = Role.GetRole<Lookout>(PlayerControl.LocalPlayer);
                var WatchButton = __instance.KillButton;

                // Check if the game state allows the KillButton to be active
                bool isKillButtonActive = __instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled;
                isKillButtonActive = isKillButtonActive && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead;
                isKillButtonActive = isKillButtonActive && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay);

                WatchButton.gameObject.SetActive(isKillButtonActive);

                var renderer = WatchButton.graphic;
                if (!WatchButton.isCoolingDown || role.IsWatching)
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                }

                if (role.ClosestPlayer != null)
                {
                    role.ClosestPlayer.myRend().material.SetColor("_OutlineColor", Palette.ImpostorRed);
                }

                if (role.IsWatching) WatchButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.WatchDuration);
                else WatchButton.SetCoolDown(role.WatchTimer(), CustomGameOptions.WatchCD);

                if (role.IsWatching && !role.WatchedPlayer.Data.IsDead)
                {
                    PlayerControl.LocalPlayer.moveable = false;
                    Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(role.WatchedPlayer);
                    var light = PlayerControl.LocalPlayer.lightSource;
                    light.transform.SetParent(role.WatchedPlayer.transform);
                    light.transform.localPosition = role.WatchedPlayer.Collider.offset;
                }
                else if (role.IsWatching)
                {
                    PlayerControl.LocalPlayer.moveable = false;
                    foreach (var body in Object.FindObjectsOfType<DeadBody>())
                    {
                        if (body.ParentId == role.WatchedPlayer.PlayerId)
                        {
                            Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(body);
                            var light = PlayerControl.LocalPlayer.lightSource;
                            light.transform.SetParent(body.transform);
                            light.transform.localPosition = body.myCollider.offset;
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class PatchWatched
        {
            public static void Postfix(HudManager __instance)
            {
                var lookouts = Role.AllRoles.Where(x => x.RoleType == RoleEnum.Lookout && x.Player != null).Cast<Lookout>();
                foreach (var role in lookouts)
                {
                    if (!MeetingHud.Instance && role.WatchedPlayer != null && !role.WatchedPlayer.Data.IsDead
                    && !role.WatchedPlayer.Data.Disconnected && role.IsWatching == true)
                    {
                        if (role.WatchedPlayer == PlayerControl.LocalPlayer && CustomGameOptions.WatchedKnows)
                        {
                            if (TaskOverlay == null)
                            {
                                TaskOverlay = Object.Instantiate(HudManager.Instance.TaskCompleteOverlay, HudManager.Instance.TaskCompleteOverlay.transform.parent);
                                TaskOverlay.gameObject.SetActive(false);
                            }
                            TaskOverlay.gameObject.GetComponentInChildren<TextMeshPro>().text = "<color=#33FF66FF>You are being Watched!</color>";
                            TaskOverlay.gameObject.transform.localPosition = new Vector3(0f, 0f, 1f);
                            TaskOverlay.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
    }
}