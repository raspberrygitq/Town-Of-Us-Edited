﻿﻿using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Extensions;
using UnityEngine;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch]
    public static class LadderFix
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetKinematic))]
        static bool Prefix(PlayerControl __instance, bool b)
        {
            if (__instance != PlayerControl.LocalPlayer) return true;
            if (!__instance.onLadder) return true;
            if (b) return true;
            var AllLadders = GameObject.FindObjectsOfType<Ladder>();
            var Ladder = AllLadders.OrderBy(x => Vector3.Distance(x.transform.position, __instance.transform.position)).ElementAt(0);
            if (__instance.GetAppearance().SizeFactor == new Vector3(1.0f, 1.0f, 1.0f))
            {
                if (!Ladder.IsTop) return true;

                __instance.NetTransform.RpcSnapTo(__instance.transform.position + new Vector3(0, 0.25f));
            }
            else if (__instance.GetAppearance().SizeFactor == new Vector3(0.4f, 0.4f, 1.0f))
            {
                if (Ladder.IsTop) return true;

                __instance.NetTransform.RpcSnapTo(__instance.transform.position + new Vector3(0, -0.25f));
            }

            return true;
        }
    }
}