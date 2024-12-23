using System.Collections;
using HarmonyLib;
using Reactor.Utilities;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.TrollMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class UpdateTroll
    {
        public static bool StartedMeeting = false;
        public static void Postfix(HudManager __instance)
        {
            if (MeetingHud.Instance || ExileController.Instance || !StartedMeeting) return;

            // Using a coroutine to make sure the code runs after the ExileController wrap up patch
            Coroutines.Start(StopTroll());
        }

        public static IEnumerator StopTroll()
        {
            yield return new WaitForSeconds(1);

            foreach (var role in Role.GetRoles(RoleEnum.Troll))
            {
                var troll = (Troll)role;
                if (troll.UsedTroll == true) troll.UsedTroll = false;
            }

            StartedMeeting = false;

            yield break;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class StartMeeting
    {
        public static void Postfix()
        {
            UpdateTroll.StartedMeeting = true;
        }
    }
}