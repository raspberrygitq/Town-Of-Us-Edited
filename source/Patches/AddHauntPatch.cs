using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
using TownOfUs.NeutralRoles.PhantomMod;
using TownOfUs.CrewmateRoles.HaunterMod;
using TownOfUs.Roles;
using TownOfUs.ImpostorRoles.SpiritMod;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipAddHauntPatch
    {
        public static void Postfix(AirshipExileController __instance) => AddHauntPatch.ExileControllerPostfix(__instance);
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
                }
                catch { }
            }
            AssassinatedPlayers.Clear();
        }

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance?.currentNormalGameOptions?.MapId != 6) return;
            if (obj.name?.Contains("ExileCutscene") == true) ExileControllerPostfix(ExileControllerPatch.lastExiled);
        }
    }
}