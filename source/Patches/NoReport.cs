using System.Linq;
using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch(typeof(HudManager))]
    public class NoReportMeetingPatch
    {
        [HarmonyPriority(Priority.Last)]
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (LobbyBehaviour.Instance) return;
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            if (AmongUsClient.Instance.IsGameOver) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.CanMove) return;
            var alives = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.Disconnected && !x.Data.IsDead).ToList();
            if (CustomGameOptions.GameMode == GameMode.BattleRoyale ||
            CustomGameOptions.GameMode == GameMode.Chaos)
            {
                foreach (var player in alives)
                {
                    if (PlayerControl.LocalPlayer == player)
                    {
                        PlayerControl.LocalPlayer.RemainingEmergencies = 0;
                        __instance.ReportButton.Hide();
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public class CantReport
        {
            public static void Postfix(PlayerControl __instance)
            {
                if (LobbyBehaviour.Instance) return;
                if (AmongUsClient.Instance.IsGameOver) return;
                if (!__instance.AmOwner) return;
                if (!__instance.CanMove) return;
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Manipulator) && Role.GetRole<Manipulator>(PlayerControl.LocalPlayer).UsingManipulation)
                {
                    DestroyableSingleton<HudManager>.Instance.ReportButton.SetActive(false);
                    return;
                }
                if (CustomGameOptions.GameMode != GameMode.BattleRoyale &&
                CustomGameOptions.GameMode != GameMode.Chaos) return;
                if (PlayerControl.LocalPlayer.Data.IsDead) return;
                var truePosition = __instance.GetTruePosition();

                var data = __instance.Data;
                var stuff = Physics2D.OverlapCircleAll(truePosition, __instance.MaxReportDistance, Constants.Usables);
                var flag = (GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks || !data.IsDead) &&
                           (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) && __instance.CanMove;
                var flag2 = false;

                foreach (var collider2D in stuff)
                    if (flag && !data.IsDead && !flag2 && collider2D.tag == "DeadBody")
                    {
                        var component = collider2D.GetComponent<DeadBody>();

                        if (Vector2.Distance(truePosition, component.TruePosition) <= __instance.MaxReportDistance)
                        {
                            if (!PhysicsHelpers.AnythingBetween(__instance.Collider, truePosition, component.TruePosition, Constants.ShipOnlyMask, false)) flag2 = true;
                        }
                    }

                DestroyableSingleton<HudManager>.Instance.ReportButton.SetActive(flag2);
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ReportClosest))]
        public static class DontReport
        {
            public static bool Prefix(PlayerControl __instance)
            {
                if (LobbyBehaviour.Instance) return false;
                if (AmongUsClient.Instance.IsGameOver) return false;
                if (PlayerControl.LocalPlayer.Data.IsDead) return false;
                if (CustomGameOptions.GameMode == GameMode.BattleRoyale
                || CustomGameOptions.GameMode == GameMode.Chaos) return false;
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Manipulator) && Role.GetRole<Manipulator>(PlayerControl.LocalPlayer).UsingManipulation) return false;
                foreach (var collider2D in Physics2D.OverlapCircleAll(__instance.GetTruePosition(),
                    __instance.MaxReportDistance, Constants.PlayersOnlyMask))
                    if (!(collider2D.tag != "DeadBody"))
                    {
                        var component = collider2D.GetComponent<DeadBody>();
                        if (component && !component.Reported)
                        {
                            component.OnClick();
                            if (component.Reported) break;
                        }
                    }

                return false;
            }
        }

        [HarmonyPatch(typeof(DeadBody), nameof(DeadBody.OnClick))]
        public static class DontClick
        {
            public static bool Prefix(DeadBody __instance)
            {
                if (LobbyBehaviour.Instance) return false;
                if (AmongUsClient.Instance.IsGameOver) return false;
                if (PlayerControl.LocalPlayer.Data.IsDead) return false;

                if (CustomGameOptions.GameMode == GameMode.BattleRoyale ||
                CustomGameOptions.GameMode == GameMode.Chaos) return false;
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Manipulator) && Role.GetRole<Manipulator>(PlayerControl.LocalPlayer).UsingManipulation) return false;
                return true;
            }
        }
    }
}