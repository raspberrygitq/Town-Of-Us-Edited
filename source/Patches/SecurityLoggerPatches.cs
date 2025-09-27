﻿using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace TownOfUsEdited.Patches;

internal static class SecurityLoggerPatches
{
    [HarmonyPatch(typeof(SecurityLogger), nameof(SecurityLogger.Awake))]
    public static class SecurityLoggerPatch
    {
        public static void Postfix(SecurityLogger __instance)
        {
            __instance.Timers = new Il2CppStructArray<float>(TownOfUsEdited.MaxPlayers);
        }
    }
}
