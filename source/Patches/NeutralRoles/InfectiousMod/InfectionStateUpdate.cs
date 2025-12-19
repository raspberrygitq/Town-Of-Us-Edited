using HarmonyLib;
using Reactor.Utilities.Extensions;
using System;
using TownOfUsEdited.CrewmateRoles.MedicMod;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Patches;
using TownOfUsEdited.Roles;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUsEdited
{    
    public class InfectionStateUpdate
    {

        [HarmonyPatch(typeof(AirshipExileController._WrapUpAndSpawn_d__11), nameof(AirshipExileController._WrapUpAndSpawn_d__11.MoveNext))]
        public static class AirshipExileController_WrapUpAndSpawn
        {
            public static void Postfix(AirshipExileController._WrapUpAndSpawn_d__11 __instance) => InfectionUpdate.ExileControllerPostfix(__instance.__4__this);
        }

        [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
        public class InfectionUpdate
        {
            public static void ExileControllerPostfix(ExileController __instance)
            {
                foreach (var role in Role.GetRoles(RoleEnum.Prosecutor))
                {
                    var prosecutor = (Prosecutor)role;
                    if (prosecutor.ProsecuteThisMeeting) return;
                }
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    var exiled = __instance.initData?.networkedPlayer;
                    PlayerControl exiledPlayer = null;
                    if (exiled != null) exiledPlayer = exiled.Object;
                    foreach (var infectious in Role.GetRoles(RoleEnum.Infectious))
                    {
                        var infectiousRole = (Infectious)infectious;
                        var role = Role.GetRole(player);
                        if (!player.Data.IsDead && role != null && role.InfectionState == 4 && !infectiousRole.Player.Data.IsDead &&
                        infectiousRole.Infected.Contains(player.PlayerId) && exiledPlayer != infectiousRole.Player && !player.Is(RoleEnum.Pestilence) && !player.Is(RoleEnum.Infectious))
                        {
                            Die(player, infectiousRole.Player);
                            role.DeathReason = DeathReasons.Infected;
                        }
                        else if ((exiledPlayer == infectiousRole.Player || player.Is(RoleEnum.Pestilence)) && infectiousRole.Infected.Contains(player.PlayerId))
                        {
                            role.InfectionState = 0;
                            infectiousRole.Infected.Remove(player.PlayerId);
                        }
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

            public static void Die(PlayerControl player, PlayerControl infectious)
            {
                var hudManager = DestroyableSingleton<HudManager>.Instance;
                var amOwner = player.AmOwner;
                player.gameObject.layer = LayerMask.NameToLayer("Ghost");
                player.Visible = false;
                if (amOwner)
                {
                    try
                        {
                            if (Minigame.Instance)
                            {
                                Minigame.Instance.Close();
                                Minigame.Instance.Close();
                            }

                            if (MapBehaviour.Instance)
                            {
                                MapBehaviour.Instance.Close();
                                MapBehaviour.Instance.Close();
                            }
                        }
                        catch
                        {
                        }
                    hudManager.ShadowQuad.gameObject.SetActive(false);
                    player.nameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                    player.RpcSetScanner(false);
                }
                var deadBody = new DeadPlayer
                {
                    PlayerId = player.PlayerId,
                    KillerId = infectious.PlayerId,
                    KillTime = DateTime.UtcNow
                };

                Murder.KilledPlayers.Add(deadBody);
                player.MyPhysics.StartCoroutine(player.KillAnimations.Random().CoPerformKill(player, player));
                if (amOwner)
                {
                    Utils.UpdateTaskText(player);
                }
            }
        }
    }
}