using System.Linq;
using HarmonyLib;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs
{
    internal static class TaskPatches
    {
        [HarmonyPatch(typeof(GameData), nameof(GameData.RecomputeTaskCounts))]
        private class GameData_RecomputeTaskCounts
        {
            private static bool Prefix(GameData __instance)
            {
                __instance.TotalTasks = 0;
                __instance.CompletedTasks = 0;
                for (var i = 0; i < __instance.AllPlayers.Count; i++)
                {
                    var playerInfo = __instance.AllPlayers.ToArray()[i];
                    if (!playerInfo.Disconnected && playerInfo.Tasks != null && playerInfo.Object &&
                        (GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks || !playerInfo.IsDead) && !playerInfo.IsImpostor() &&
                        !(
                            playerInfo._object.Is(RoleEnum.Jester) || playerInfo._object.Is(RoleEnum.Amnesiac) ||
                            playerInfo._object.Is(RoleEnum.Survivor) || playerInfo._object.Is(RoleEnum.GuardianAngel) ||
                            playerInfo._object.Is(RoleEnum.Glitch) || playerInfo._object.Is(RoleEnum.Executioner) ||
                            playerInfo._object.Is(RoleEnum.Arsonist) || playerInfo._object.Is(RoleEnum.Juggernaut) ||
                            playerInfo._object.Is(RoleEnum.Plaguebearer) || playerInfo._object.Is(RoleEnum.Pestilence) ||
                            playerInfo._object.Is(RoleEnum.Maul) || playerInfo._object.Is(RoleEnum.Doomsayer) ||
                            playerInfo._object.Is(RoleEnum.Vampire) || playerInfo._object.Is(RoleEnum.SerialKiller) ||
                            playerInfo._object.Is(RoleEnum.Phantom) || playerInfo._object.Is(RoleEnum.Haunter) ||
                            playerInfo._object.Is(RoleEnum.Mutant) || playerInfo._object.Is(RoleEnum.Shifter) ||
                            playerInfo._object.Is(RoleEnum.Troll) || playerInfo._object.Is(Faction.Madmates) ||
                            playerInfo._object.Is(RoleEnum.WhiteWolf) || playerInfo._object.Is(RoleEnum.Attacker) ||
                            playerInfo._object.Is(RoleEnum.Terrorist) || playerInfo._object.Is(RoleEnum.Vulture) ||
                            playerInfo._object.Is(Faction.Coven) || playerInfo._object.Is(RoleEnum.Infectious) ||
                            playerInfo._object.Is(RoleEnum.Doppelganger) || playerInfo._object.Is(Faction.Impostors) ||
                            playerInfo._object.Is(RoleEnum.Spectator)
                        ))
                        for (var j = 0; j < playerInfo.Tasks.Count; j++)
                        {
                            __instance.TotalTasks++;
                            if (playerInfo.Tasks.ToArray()[j].Complete) __instance.CompletedTasks++;
                        }
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
        private class Console_CanUse
        {
            private static bool Prefix(Console __instance, [HarmonyArgument(0)] NetworkedPlayerInfo playerInfo, ref float __result, ref bool canUse, ref bool couldUse)
            {
                var playerControl = playerInfo.Object;

                var flag = playerControl.Is(RoleEnum.Glitch)
                           || playerControl.Is(RoleEnum.Jester)
                           || playerControl.Is(RoleEnum.Executioner)
                           || playerControl.Is(RoleEnum.Juggernaut)
                           || playerControl.Is(RoleEnum.Arsonist)
                           || playerControl.Is(RoleEnum.Plaguebearer)
                           || playerControl.Is(RoleEnum.Pestilence)
                           || playerControl.Is(RoleEnum.Maul)
                           || playerControl.Is(RoleEnum.Doomsayer)
                           || playerControl.Is(RoleEnum.Vampire)
                           || playerControl.Is(RoleEnum.Mutant)
                           || playerControl.Is(RoleEnum.Infectious)
                           || playerControl.Is(RoleEnum.SerialKiller)
                           || playerControl.Is(RoleEnum.Doppelganger)
                           || playerControl.Is(RoleEnum.Troll)
                           || (playerControl.Is(Faction.Madmates) && !playerControl.Is(RoleEnum.Snitch))
                           || (playerControl.Is(Faction.Crewmates) && CustomGameOptions.GameMode == GameMode.Werewolf)
                           || playerControl.Is(RoleEnum.WhiteWolf)
                           || playerControl.Is(RoleEnum.Terrorist)
                           || playerControl.Is(RoleEnum.Player)
                           || playerControl.Is(RoleEnum.Vulture)
                           || (playerControl.Is(Faction.Impostors) && !playerControl.Is(RoleEnum.Spirit))
                           || playerControl.Is(Faction.Coven)
                           || playerControl.Is(RoleEnum.Spectator);

                var flag2 = playerControl.Is(RoleEnum.Spirit);

                // If the console is not a sabotage repair console
                if (flag && !__instance.AllowImpostor)
                {
                    __result = float.MaxValue;
                    couldUse = false;
                    canUse = false;
                    return false;
                }

                if (flag2)
                {
                    couldUse = __instance.FindTask(playerControl) && (!__instance.onlyFromBelow || playerControl.GetTruePosition().y < __instance.transform.position.y);
                    canUse = couldUse;
                    __result = Vector2.Distance(playerControl.GetTruePosition(), __instance.transform.position);
                    canUse &= (__result <= __instance.UsableDistance);
				    canUse &= !PhysicsHelpers.AnythingBetween(playerControl.GetTruePosition(), __instance.transform.position, Constants.ShadowMask, false);
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
        public class CompleteTask
        {
            public static void Postfix(PlayerControl __instance)
            {
                if (__instance.Is(RoleEnum.Haunter) || !__instance.Is(Faction.Crewmates)) GameData.Instance.CompletedTasks--;
            }
        }
    }
}