using System;
using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsEdited.Patches;

namespace TownOfUsEdited.ImpostorRoles.BlinderMod
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => SetBlinder.ExileControllerPostfix(__instance);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class SetBlinder
    {
        public static PlayerControl WillBeBlinder;

        public static void ExileControllerPostfix(ExileController __instance)
        {
            if (WillBeBlinder == null) return;
            if (WillBeBlinder.Data.Disconnected) return;

            if (!WillBeBlinder.Is(RoleEnum.Blinder))
            {
                var oldRole = Role.GetRole(WillBeBlinder);
                var killsList = (oldRole.CorrectKills, oldRole.IncorrectKills, oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
                Role.RoleDictionary.Remove(WillBeBlinder.PlayerId);
                if (PlayerControl.LocalPlayer == WillBeBlinder)
                {
                    var role = new Blinder(PlayerControl.LocalPlayer);
                    role.formerRole = oldRole.RoleType;
                    role.CorrectKills = killsList.CorrectKills;
                    role.IncorrectKills = killsList.IncorrectKills;
                    role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                    role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                    role.DeathReason = oldRole.DeathReason;
                    role.RegenTask();
                }
                else
                {
                    var role = new Blinder(WillBeBlinder);
                    role.formerRole = oldRole.RoleType;
                    role.CorrectKills = killsList.CorrectKills;
                    role.IncorrectKills = killsList.IncorrectKills;
                    role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                    role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                    role.DeathReason = oldRole.DeathReason;
                    //Not necessary to regen task here, I only do it so it looks good on mci lol
                    role.RegenTask();
                }
            }
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