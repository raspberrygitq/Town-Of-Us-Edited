using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUsEdited.CrewmateRoles.MedicMod;
using TownOfUsEdited.Roles;
using UnityEngine;
using TownOfUsEdited.Roles.Modifiers;
using AmongUs.GameOptions;
using TownOfUsEdited.Patches;

namespace TownOfUsEdited.CrewmateRoles.DoctorMod
{
    public class DocRevive
    {
        public static void StopDragging(byte playerId)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Undertaker))
            {
                var undertaker = (Undertaker)role;
                if (undertaker.CurrentlyDragging != null && undertaker.CurrentlyDragging.ParentId == playerId)
                {
                    Vector3 position = undertaker.Player.transform.position;

                    if (Patches.SubmergedCompatibility.isSubmerged())
                    {
                        if (position.y > -7f)
                        {
                            position.z = 0.0208f;
                        }
                        else
                        {
                            position.z = -0.0273f;
                        }
                    }

                    position.y -= 0.3636f;

                    var body = undertaker.CurrentlyDragging;
                    if (undertaker.Player == PlayerControl.LocalPlayer)
                    {
                        foreach (var body2 in undertaker.CurrentlyDragging.bodyRenderers) body2.material.SetFloat("_Outline", 0f);
                    }
                    undertaker.CurrentlyDragging = null;

                    body.transform.position = position;
                }
            }
            foreach (var role in Role.GetRoles(RoleEnum.Doctor))
            {
                var doctor = (Doctor)role;
                if (doctor.CurrentlyDragging != null && doctor.CurrentlyDragging.ParentId == playerId)
                {
                    Vector3 position = doctor.Player.transform.position;

                    if (Patches.SubmergedCompatibility.isSubmerged())
                    {
                        if (position.y > -7f)
                        {
                            position.z = 0.0208f;
                        }
                        else
                        {
                            position.z = -0.0273f;
                        }
                    }

                    position.y -= 0.3636f;

                    var body = doctor.CurrentlyDragging;
                    if (doctor.Player == PlayerControl.LocalPlayer)
                    {
                        foreach (var body2 in doctor.CurrentlyDragging.bodyRenderers) body2.material.SetFloat("_Outline", 0f);
                    }
                    doctor.CurrentlyDragging = null;

                    body.transform.position = position;
                }
            }
        }
        public static void DoctorRevive(DeadBody target, Doctor role)
        {
            var parentId = target.ParentId;
            StopDragging(parentId);
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
                if (poisonerRole.PoisonedPlayer == player) poisonerRole.PoisonedPlayer = null;
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
            var usedPosition = new Vector2(position.x, position.y + 0.3636f);
            player.transform.position = new Vector2(usedPosition.x, usedPosition.y);

            if (Patches.SubmergedCompatibility.isSubmerged() && PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
            {
                Patches.SubmergedCompatibility.ChangeFloor(player.transform.position.y > -7);
            }

            if (player.IsLover() && CustomGameOptions.BothLoversDie)
            {
                var lover = Modifier.GetModifier<Lover>(player).OtherLover.Player;

                foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
                {
                    if (deadBody.ParentId == lover.PlayerId)
                    {
                        lover.Revive();
                        RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);
                        Murder.KilledPlayers.Remove(
                            Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == lover.PlayerId));
                        revived.Add(lover);

                        var position2 = deadBody.TruePosition;
                        var usedPosition2 = new Vector2(position2.x, position2.y + 0.3636f);
                        lover.transform.position = new Vector2(usedPosition2.x, usedPosition2.y);

                        if (Patches.SubmergedCompatibility.isSubmerged() && PlayerControl.LocalPlayer.PlayerId == lover.PlayerId)
                        {
                            Patches.SubmergedCompatibility.ChangeFloor(lover.transform.position.y > -7);
                        }
                        deadBody.gameObject.Destroy();
                    }
                }
            }
            if (player.Is(ModifierEnum.Celebrity))
            {
                var celeb = Modifier.GetModifier<Celebrity>(player);
                celeb.JustDied = false;
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
                PlayerControl.LocalPlayer.myTasks.RemoveAt(1);
            }
        }
    }
}