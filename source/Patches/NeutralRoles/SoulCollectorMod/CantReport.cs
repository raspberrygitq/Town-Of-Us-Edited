using System.Linq;
using HarmonyLib;
using TownOfUsEdited.CrewmateRoles.MedicMod;
using UnityEngine;

namespace TownOfUsEdited.NeutralRoles.SoulCollectorMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class CantReport
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.AmOwner) return;
            if (!__instance.CanMove) return;
            if (!__instance.Is(RoleEnum.SoulCollector)) return;
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
                        var matches = Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == component.ParentId);
                        if (matches != null && matches.KillerId != PlayerControl.LocalPlayer.PlayerId) { 
                            if (!PhysicsHelpers.AnythingBetween(__instance.Collider, truePosition, component.TruePosition, Constants.ShipOnlyMask, false)) flag2 = true; 
                        }
                    }
                }

            HudManager.Instance.ReportButton.SetActive(flag2);
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ReportClosest))]
    public static class DontReport
    {
        public static bool Prefix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.SoulCollector)) return true;

            if (AmongUsClient.Instance.IsGameOver) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            foreach (var collider2D in Physics2D.OverlapCircleAll(__instance.GetTruePosition(),
                __instance.MaxReportDistance, Constants.PlayersOnlyMask))
                if (!(collider2D.tag != "DeadBody"))
                {
                    var component = collider2D.GetComponent<DeadBody>();
                    if (component && !component.Reported)
                    {
                        var matches = Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == component.ParentId);
                        if (matches != null && matches.KillerId != PlayerControl.LocalPlayer.PlayerId)
                            component.OnClick();
                        if (component.Reported) break;
                    }
                }

            return false;
        }
    }
}