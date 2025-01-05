using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUs.Roles.Modifiers;
using AmongUs.GameOptions;
using TownOfUs.Patches;

namespace TownOfUs.CrewmateRoles.DoctorMod
{
    public class DocRevive
    {
        public static void DoctorRevive(DeadBody target, Doctor role)
        {
            var parentId = target.ParentId;
            var position = target.TruePosition;

            if (target.IsDouble())
            {
                var matches = Murder.KilledPlayers.ToArray().Where(x => x.KillerId == target.ParentId && x.isDoppel == true).ToList();
                if (matches.Any())
                {
                    foreach (var killedPlayer in matches)
                    {
                        parentId = killedPlayer.PlayerId;
                    }
                }
            }

            var revived = role.RevivedList;

                if (target != null)
                {
                    foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
                    {
                        if (deadBody.ParentId == target.ParentId) deadBody.gameObject.Destroy();
                    }
                }

            var player = Utils.PlayerById(parentId);

            foreach (var poisoner in Role.GetRoles(RoleEnum.Poisoner))
            {
                var poisonerRole = (Poisoner)poisoner;
                if (poisonerRole.PoisonedPlayer == player) poisonerRole.PoisonedPlayer = poisonerRole.Player;
            }

            if (CustomGameOptions.GameMode != GameMode.Chaos)
            {
                Utils.Rpc(CustomRPC.DoctorPopUp, player.PlayerId);
            }

            player.Revive();
            RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);
            Murder.KilledPlayers.Remove(
                Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == player.PlayerId));
            revived.Add(player);
            player.NetTransform.SnapTo(new Vector2(position.x, position.y + 0.3636f));

            if (Patches.SubmergedCompatibility.isSubmerged() && PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
            {
                Patches.SubmergedCompatibility.ChangeFloor(player.transform.position.y > -7);
            }
            if (target != null) Object.Destroy(target.gameObject);

            if (player.IsLover() && CustomGameOptions.BothLoversDie)
            {
                var lover = Modifier.GetModifier<Lover>(player).OtherLover.Player;

                lover.Revive();
                RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);
                Murder.KilledPlayers.Remove(
                    Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == lover.PlayerId));
                revived.Add(lover);

                if (PlayerControl.LocalPlayer == lover) lover.myTasks.RemoveAt(1);

                foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
                {
                    if (deadBody.ParentId == lover.PlayerId)
                    {
                        var position2 = deadBody.TruePosition;
                        lover.NetTransform.SnapTo(new Vector2(position2.x, position2.y + 0.3636f));

                        if (Patches.SubmergedCompatibility.isSubmerged() && PlayerControl.LocalPlayer.PlayerId == lover.PlayerId)
                        {
                            Patches.SubmergedCompatibility.ChangeFloor(lover.transform.position.y > -7);
                        }
                        deadBody.gameObject.Destroy();
                    }
                }
            }

            if (revived.Any(x => x.AmOwner && !x.Data.IsDead))
                try
                {
                    Minigame.Instance.Close();
                    Minigame.Instance.Close();
                }
                catch
                {
                }

            if (PlayerControl.LocalPlayer == player)
            {
                Utils.FlashCoroutine(Colors.Doctor, 1f, 0.5f);
                player.myTasks.RemoveAt(1);
            }
        }
    }
}