using HarmonyLib;
using System;
using System.Linq;

namespace TownOfUsEdited.Patches
{
    //Code given by Det, thanks to him (:
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ReportDeadBody))]
    public static class TaskMeetingOverride
    { 
        public static bool Prefix(PlayerControl __instance, ref NetworkedPlayerInfo target)
        {
            var totalCrew = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Data.Disconnected).ToList();

            if (AmongUsClient.Instance.IsGameOver)
            {
                return false;
            }
            if (MeetingHud.Instance)
            {
                return false;
            }
            if (target == null && PlayerControl.LocalPlayer.myTasks.Find(new Func<PlayerTask, bool>(PlayerTask.TaskIsEmergency)))
            {
                return false;
            }
            if (__instance.Data.IsDead)
            {
                return false;
            }
            MeetingRoomManager.Instance.AssignSelf(__instance, target);
            if (!AmongUsClient.Instance.AmHost)
            {
                return false;
            }
            if (totalCrew.Count != 0)
            {
                if (GameManager.Instance.CheckTaskCompletion())
                {
                    return false;
                }
            }
            if (target == null)
            {
                __instance.logger.Debug("Calling emergency meeting", null);
            }
            else
            {
                __instance.logger.Debug(string.Format("Reporting dead body {0}", target.PlayerId), null);
            }
            DestroyableSingleton<HudManager>.Instance.OpenMeetingRoom(__instance);
            __instance.RpcStartMeeting(target);
            return false;
        }
    }
}