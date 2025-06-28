using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using Reactor.Utilities.Extensions;
using TownOfUsEdited.CrewmateRoles.MedicMod;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Roles.Modifiers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUsEdited.Roles
{
    public class Reviver : Role, IVisualAlteration
    {
        public PlayerControl RevivedPlayer;
        public KillButton _reviveButton;
        public bool UsedRevive = false;
        public bool CanRevive = true;
        public DeadBody CurrentTarget;

        public Reviver(PlayerControl player) : base(player)
        {
            Name = "Reviver";
            ImpostorText = () => "live... or die.";
            TaskText = () => "Revive inside of a dead player\nFake Tasks:";
            Color = Patches.Colors.Impostor;
            RoleType = RoleEnum.Reviver;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            Alignment = Alignment.ImpostorSupport;
        }

        public KillButton ReviveButton
        {
            get => _reviveButton;
            set
            {
                _reviveButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            if (UsedRevive)
            {
                appearance = RevivedPlayer.GetDefaultAppearance();
                var modifiers = Modifier.GetModifiers(RevivedPlayer);
                var modifier = modifiers.FirstOrDefault(x => x is IVisualAlteration);
                if (modifier is IVisualAlteration alteration)
                    alteration.TryGetModifiedAppearance(out appearance);
                return true;
            }

            appearance = Player.GetDefaultAppearance();
            return false;
        }

        public void ReviveAbility(DeadBody target)
        {
            Revive(target, PlayerControl.LocalPlayer);
            Utils.Rpc(CustomRPC.ReviverRevive, PlayerControl.LocalPlayer.PlayerId, target.ParentId);
        }

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

        public static void Revive(DeadBody target, PlayerControl player)
        {
            var playerId = target.ParentId;
            var player2 = Utils.PlayerById(playerId);
            StopDragging(playerId);
            var position = target.TruePosition;
            var role = Role.GetRole<Reviver>(player);
            role.UsedRevive = true;
            role.CanRevive = false;

            var revived = new List<PlayerControl>();

            if (target != null)
            {
                foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
                {
                    if (deadBody.ParentId == target.ParentId) deadBody.gameObject.Destroy();
                }
            }

            player.Revive();
            Murder.KilledPlayers.Remove(
                Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == player.PlayerId));
            revived.Add(player);
            var usedPosition = new Vector2(position.x, position.y + 0.3636f);
            player.transform.position = new Vector2(usedPosition.x, usedPosition.y);
            RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);

            if (PlayerControl.LocalPlayer == player) player.myTasks.RemoveAt(1);

            if (Patches.SubmergedCompatibility.isSubmerged() && PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
            {
                Patches.SubmergedCompatibility.ChangeFloor(target.transform.position.y > -7);
            }
            if (target != null) Object.Destroy(target.gameObject);

            if (revived.Any(x => x.AmOwner))
                try
                {
                    Minigame.Instance.Close();
                    Minigame.Instance.Close();
                }
                catch
                {
                }
            Utils.Unmorph(player);
            Utils.Morph(player, player2);
            role.RevivedPlayer = player2;
            return;
        }
    }
}