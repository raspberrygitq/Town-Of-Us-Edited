using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TownOfUsEdited.Patches;
using TownOfUsEdited.Roles;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace TownOfUsEdited.ImpostorRoles.WraithMod
{
    [HarmonyPatch(typeof(AirshipExileController._WrapUpAndSpawn_d__11), nameof(AirshipExileController._WrapUpAndSpawn_d__11.MoveNext))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController._WrapUpAndSpawn_d__11 __instance) => SetWraith.ExileControllerPostfix(__instance.__4__this);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class SetWraith
    {
        public static PlayerControl WillBeWraith;
        public static Vector2 StartPosition;

        public static void ExileControllerPostfix(ExileController __instance)
        {
            if (WillBeWraith == null) return;
            if (WillBeWraith.Data.Disconnected) return;

            if (!WillBeWraith.Is(RoleEnum.Wraith))
            {
                var oldRole = Role.GetRole(WillBeWraith);
                var killsList = (oldRole.Kills, oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
                Role.RoleDictionary.Remove(WillBeWraith.PlayerId);
                if (PlayerControl.LocalPlayer == WillBeWraith)
                {
                    var role = new Wraith(PlayerControl.LocalPlayer);
                    role.formerRole = oldRole.RoleType;
                    role.Kills = killsList.Kills;
                    role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                    role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                    role.DeathReason = oldRole.DeathReason;
                    role.RegenTask();
                }
                else
                {
                    var role = new Wraith(WillBeWraith);
                    role.formerRole = oldRole.RoleType;
                    role.Kills = killsList.Kills;
                    role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                    role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                    role.DeathReason = oldRole.DeathReason;
                    // Not necessary to regen task here, I only do it so it looks good on mci lol
                    role.RegenTask();
                }

                Utils.RemoveTasks(WillBeWraith);
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Haunter) && !PlayerControl.LocalPlayer.Is(RoleEnum.Phantom)) WillBeWraith.MyPhysics.ResetMoveState();

                WillBeWraith.gameObject.layer = LayerMask.NameToLayer("Players");
            }

            WillBeWraith.gameObject.GetComponent<PassiveButton>().OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            WillBeWraith.gameObject.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() => WillBeWraith.OnClick()));
            WillBeWraith.gameObject.GetComponent<BoxCollider2D>().enabled = true;

            if (PlayerControl.LocalPlayer != WillBeWraith) return;

            if (Role.GetRole<Wraith>(PlayerControl.LocalPlayer).Caught) return;

            List<Vent> vents = new();
            var CleanVentTasks = PlayerControl.LocalPlayer.myTasks.ToArray().Where(x => x.TaskType == TaskTypes.VentCleaning).ToList();
            if (CleanVentTasks != null)
            {
                var ids = CleanVentTasks.Where(x => !x.IsComplete)
                                        .ToList()
                                        .ConvertAll(x => x.FindConsoles()[0].ConsoleId);

                vents = ShipStatus.Instance.AllVents.Where(x => !ids.Contains(x.Id)).ToList();
            }
            else vents = ShipStatus.Instance.AllVents.ToList();

            var startingVent = vents[Random.RandomRangeInt(0, vents.Count)];


            Utils.Rpc(CustomRPC.SetPos, PlayerControl.LocalPlayer.PlayerId, startingVent.transform.position.x, startingVent.transform.position.y + 0.3636f);
            var pos = new Vector2(startingVent.transform.position.x, startingVent.transform.position.y + 0.3636f);

            PlayerControl.LocalPlayer.transform.position = pos;
            PlayerControl.LocalPlayer.NetTransform.SnapTo(pos);
            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(pos);
            PlayerControl.LocalPlayer.MyPhysics.RpcEnterVent(startingVent.Id);
        }

        [HarmonyPriority(Priority.First)]
        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance?.currentNormalGameOptions?.MapId != 6) return;
            if (obj.name?.Contains("ExileCutscene") == true) ExileControllerPostfix(ExileControllerPatch.lastExiled);
        }
    }
}