using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.AstralMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class CantReport
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.AmOwner) return;
            if (!__instance.CanMove) return;
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
                        if (component.IsDouble())
                        {
                            if (!PhysicsHelpers.AnythingBetween(__instance.Collider, truePosition, component.TruePosition, Constants.ShipOnlyMask, false))
                            {
                                DestroyableSingleton<HudManager>.Instance.ReportButton.SetActive(true);
                            }
                            return;
                        }
                        var matches = PlayerControl.AllPlayerControls.ToArray().Where(x => x.PlayerId == component.ParentId).ToList();
                        if (matches.Any())
                        {
                            foreach (var player in matches)
                            {
                                if (player.Is(RoleEnum.Astral) && Role.GetRole<Astral>(player).Enabled == true)
                                {
                                    DestroyableSingleton<HudManager>.Instance.ReportButton.SetActive(false);
                                    return;
                                }
                            }
                        }
                        
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
            if (AmongUsClient.Instance.IsGameOver) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            foreach (var collider2D in Physics2D.OverlapCircleAll(__instance.GetTruePosition(),
                __instance.MaxReportDistance, Constants.PlayersOnlyMask))
                if (!(collider2D.tag != "DeadBody"))
                {
                    var component = collider2D.GetComponent<DeadBody>();
                    if (component && !component.Reported)
                    {
                        if (component.IsDouble()) return true;
                        var matches = PlayerControl.AllPlayerControls.ToArray().Where(x => x.PlayerId == component.ParentId).ToList();
                        if (matches.Any())
                        {
                            foreach (var player in matches)
                            {
                                if (player.Is(RoleEnum.Astral) && Role.GetRole<Astral>(player).Enabled == true)
                                {
                                    return false;
                                }
                            }
                        }
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
            if (AmongUsClient.Instance.IsGameOver) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;

            if (__instance.IsDouble()) return true;

            var matches = PlayerControl.AllPlayerControls.ToArray().Where(x => x.PlayerId == __instance.ParentId).ToList();
            if (matches.Any())
            {
                foreach (var player in matches)
                {
                    if (player.Is(RoleEnum.Astral) && Role.GetRole<Astral>(player).Enabled == true)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
