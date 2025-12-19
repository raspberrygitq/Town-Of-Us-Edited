using AmongUs.GameOptions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using TownOfUsEdited.CrewmateRoles.HaunterMod;
using TownOfUsEdited.ImpostorRoles.SpiritMod;
using TownOfUsEdited.NeutralRoles.PhantomMod;
using TownOfUsEdited.Roles;
using TownOfUsEdited.Roles.Modifiers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch(typeof(AirshipExileController._WrapUpAndSpawn_d__11), nameof(AirshipExileController._WrapUpAndSpawn_d__11.MoveNext))]
    public static class AirshipAddHauntPatch
    {
        public static void Postfix(AirshipExileController._WrapUpAndSpawn_d__11 __instance) => AddHauntPatch.ExileControllerPostfix(__instance.__4__this);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    [HarmonyPriority(Priority.First)]
    class AddHauntPatch
    {
        public static List<PlayerControl> AssassinatedPlayers = new List<PlayerControl>();

        public static void ExileControllerPostfix(ExileController __instance)
        {
            foreach (var player in AssassinatedPlayers)
            {
                foreach (var role in Role.GetRoles(RoleEnum.Reviver))
                {
                    var reviver = (Reviver)role;
                    if (reviver.UsedRevive && player == reviver.Player)
                    {
                        Utils.Unmorph(reviver.Player);
                    }
                    else if (!reviver.UsedRevive && player == reviver.Player)
                    {
                        reviver.CanRevive = false;
                    }
                }
                try
                {
                    if (SetPhantom.WillBePhantom != player && SetHaunter.WillBeHaunter != player &&
                    SetSpirit.WillBeSpirit != player && !player.Data.Disconnected) player.Exiled();
                    if (AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay)
                    {
                        RoleManager.Instance.SetRole(player, RoleTypes.CrewmateGhost);
                        if (player == PlayerControl.LocalPlayer) Utils.ShowDeadBodies = true;
                    }
                }
                catch { }
            }
            AssassinatedPlayers.Clear();
            if (GameOptionsManager.Instance.currentNormalGameOptions?.MapId != 6 && GameOptionsManager.Instance.currentNormalGameOptions?.MapId != 4 && PlayerControl.LocalPlayer.Is(ModifierEnum.Motionless))
            {
                var role = Modifier.GetModifier<Motionless>(PlayerControl.LocalPlayer);
                role.ResetPosition();
            }
        }

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance?.currentNormalGameOptions?.MapId != 6) return;
            if (obj.name?.Contains("ExileCutscene") == true) ExileControllerPostfix(ExileControllerPatch.lastExiled);
        }

        // Code from Stellar roles, link: https://github.com/Mr-Fluuff/StellarRolesAU/blob/bad6c0e70557897021fc9b257588b32e29b705b9/StellarRoles/Patches/ExileControllerPatch.cs#L77
        [HarmonyPatch(typeof(UnityEngine.Object), nameof(UnityEngine.Object.Destroy), [typeof(GameObject)])]
        class SubmergedExileControllerPatch
        {
            public static void Prefix(GameObject obj, ref bool __state)
            {
                if (!SubmergedCompatibility.isSubmerged() || GameOptionsManager.Instance.currentNormalGameOptions?.MapId != 6) return;
                try
                {
                    if (obj?.name?.Contains("SpawnInMinigame") == true)
                    {
                        __state = true;
                    }
                }
                catch { }
            }
            public static void Postfix(bool __state)
            {
                if (__state)
                {
                    if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Motionless)) return;
                    var role = Modifier.GetModifier<Motionless>(PlayerControl.LocalPlayer);
                    role.ResetPosition();
                }
            }
        }
    }
}