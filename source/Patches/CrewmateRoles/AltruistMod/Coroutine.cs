using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUsEdited.CrewmateRoles.MedicMod;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Roles;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsEdited.Roles.Modifiers;
using AmongUs.GameOptions;
using TownOfUsEdited.Patches;

namespace TownOfUsEdited.CrewmateRoles.AltruistMod
{
    public class Coroutine
    {
        public static Dictionary<PlayerControl, ArrowBehaviour> Revived = new();
        public static Sprite Sprite => TownOfUsEdited.Arrow;
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
        public static IEnumerator AltruistRevive(DeadBody target, Altruist role)
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

            var revived = new List<PlayerControl>();

            if (AmongUsClient.Instance.AmHost) Utils.RpcMurderPlayer(role.Player, role.Player);

            if (CustomGameOptions.AltruistTargetBody)
                if (target != null)
                {
                    foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
                    {
                        if (deadBody.ParentId == target.ParentId) deadBody.gameObject.Destroy();
                    }
                }

            var startTime = DateTime.UtcNow;
            while (true)
            {
                var now = DateTime.UtcNow;
                var seconds = (now - startTime).TotalSeconds;
                if (seconds < CustomGameOptions.ReviveDuration)
                    yield return null;
                else break;

                if (MeetingHud.Instance) yield break;
            }

            foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
            {
                if (deadBody.ParentId == role.Player.PlayerId) deadBody.gameObject.Destroy();
            }

            var player = Utils.PlayerById(parentId);

            foreach (var poisoner in Role.GetRoles(RoleEnum.Poisoner))
            {
                var poisonerRole = (Poisoner)poisoner;
                if (poisonerRole.PoisonedPlayer == player) poisonerRole.PoisonedPlayer = null;
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
            if (target != null) Object.Destroy(target.gameObject);

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

            if (revived.Any(x => x.AmOwner))
                try
                {
                    Minigame.Instance.Close();
                    Minigame.Instance.Close();
                }
                catch
                {
                }

            if (PlayerControl.LocalPlayer == player) player.myTasks.RemoveAt(1);

            if (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(Faction.NeutralKilling))
            {
                var gameObj = new GameObject();
                var Arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = Sprite;
                Arrow.image = renderer;
                gameObj.layer = 5;
                Revived.Add(player, Arrow);
                //Target = player;
                yield return Utils.FlashCoroutine(Colors.Altruist, 1f, 0.5f);
            }
        }
    }
}