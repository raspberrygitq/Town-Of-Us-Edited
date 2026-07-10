using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.Modules;

public static class LevelImpostorCompatibility
{
    public const string LiGuid = "com.DigiWorm.LevelImposter";

    public static bool Loaded { get; private set; }
    public static BasePlugin Plugin { get; private set; }
    public static Assembly Assembly { get; private set; }
    private static Dictionary<string, Type> Types { get; set; }

    public static void Initialize()
    {
        Loaded = IL2CPPChainloader.Instance.Plugins.TryGetValue(LiGuid, out PluginInfo liPlugin);
        if (!Loaded) return;

        Plugin = liPlugin.Instance as BasePlugin;

        Assembly = Plugin!.GetType().Assembly;
        Types = AccessTools.GetTypesFromAssembly(Assembly).TryToDictionary(x => x.Name, x => x);

        var canUseMethod = AccessTools.Method(Types["TriggerConsole"], "CanUse");

        var compatType = typeof(LevelImpostorCompatibility);

        var _harmony = new Harmony("toue.levelimpostor.patch");
        _harmony.Patch(canUseMethod, new(AccessTools.Method(compatType, nameof(TriggerPrefix))), new(AccessTools.Method(compatType, nameof(TriggerPostfix))));
    }

    public static void TriggerPrefix(NetworkedPlayerInfo playerInfo, ref bool __state)
    {
        var playerControl = playerInfo.Object;
        bool isGhostRole = (playerControl.Is(RoleEnum.Haunter) && !Role.GetRole<Haunter>(PlayerControl.LocalPlayer).Caught) ||
        (playerControl.Is(RoleEnum.Phantom) && !Role.GetRole<Phantom>(PlayerControl.LocalPlayer).Caught) ||
        (playerControl.Is(RoleEnum.Wraith) && !Role.GetRole<Wraith>(PlayerControl.LocalPlayer).Caught);

        if (isGhostRole && playerInfo.IsDead)
            return;

        playerInfo.IsDead = false;
        __state = true;
    }

    public static void TriggerPostfix(NetworkedPlayerInfo playerInfo, ref bool __state)
    {
        if (__state)
            playerInfo.IsDead = true;
    }
}
