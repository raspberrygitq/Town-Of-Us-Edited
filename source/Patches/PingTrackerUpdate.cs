using HarmonyLib;
using UnityEngine;

namespace TownOfUs
{
    //[HarmonyPriority(Priority.VeryHigh)] // to show this message first, or be overrided if any plugins do
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static class PingTracker_Update
    {
        [HarmonyPostfix]
        public static void Postfix(PingTracker __instance)
        {
            var position = __instance.GetComponent<AspectPosition>();
            position.Alignment = AspectPosition.EdgeAlignments.Top;
            position.DistanceFromEdge = new Vector3(0f, 0.1f, 0);
            position.AdjustPosition();
            var host = GameData.Instance.GetHost();

            __instance.text.text =
                "<size=2><color=#00FF00FF><color=#EE9D01>TownOfUs</color><b><color=#AA00FF>Edited</color></b> v" + TownOfUs.VersionString + "</color>" + TownOfUs.VersionTag + "\n" +
                $"Ping: {AmongUsClient.Instance.Ping}ms\n" +
                (!MeetingHud.Instance
                    ? "<color=#00FF00FF>Modded By: <color=#FF0000>le killer</color> </color>\n" +
                    "<color=#00FF00FF>help from: <color=#FFBAEA>Windyways</color>, <color=#2FD6AF>Gun</color>, <color=#9900ff>whichTwix</color> & <color=#D16002>Det</color></color>\n" : "") +
                (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started
                    ? "<color=#00FF00FF>Formerly: <color=#1B01EE>Slushiegoose & Polus.gg</color></color>\n" +
                     $"Host: {host?.PlayerName}" : "") +
                    "</size>";
            if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started && CustomGameOptions.GameMode == GameMode.Werewolf)
            {
                if (!MeetingHud.Instance) 
                {
                    __instance.text.text += $"\n<size=5><color=#922058>          Night {DayNightMechanic.NightCount}</color></size>";
                }
                else if (MeetingHud.Instance) 
                {
                    __instance.text.text += $"\n<size=5><color=#ffff00>          Day {DayNightMechanic.DayCount}</color></size>";
                }
            }
        }
    }
}
