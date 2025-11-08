using AmongUs.GameOptions;
using Reactor.Utilities.Extensions;
using System.Collections.Generic;
using System.Linq;
using TownOfUsEdited.CrewmateRoles.MedicMod;
using TownOfUsEdited.Roles.Modifiers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUsEdited.Roles
{
    public class Converter : Role
    {
        public Converter(PlayerControl owner) : base(owner)
        {
            Name = "Converter";
            ImpostorText = () => "Convert someone to your side";
            TaskText = () => "Convert an innocent to help you killing\nFake Tasks:";
            Color = Patches.Colors.Impostor;
            RoleType = RoleEnum.Converter;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            Alignment = Alignment.ImpostorSupport;
            Cooldown = CustomGameOptions.ConvertCD;
        }

        public DeadBody CurrentTarget;
        public KillButton _convertButton;
        public bool AbilityUsed { get; set; } = false;
        public KillButton ConvertButton
        {
            get => _convertButton;
            set
            {
                _convertButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public int ReviveCount = 0;
        public float ConvertTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }
        public void ConvertAbility(DeadBody target)
        {
            var player = Utils.PlayerById(target.ParentId);

            // Check if the Converter can use the ability
            if (!player.Is(Faction.Crewmates))
                return;

            if (CustomGameOptions.GameMode != GameMode.Cultist)
            {
                AbilityUsed = true;
            }

            Revive(target);
            Utils.Rpc(CustomRPC.ConverterRevive, PlayerControl.LocalPlayer.PlayerId, target.ParentId);

            // Set the last ability use time
            Cooldown = CustomGameOptions.ConvertCD + ReviveCount * 5f;
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

        public static void Revive(DeadBody target)
        {
            var playerId = target.ParentId;
            var player = Utils.PlayerById(playerId);
            StopDragging(playerId);
            var position = target.TruePosition;

            if (target.IsDouble())
            {
                var matches = Murder.KilledPlayers.ToArray().Where(x => x.KillerId == target.ParentId && x.isDoppel == true).ToList();
                if (matches.Any())
                {
                    foreach (var killedPlayer in matches)
                    {
                        playerId = killedPlayer.PlayerId;
                    }
                }
            }

            var revived = new List<PlayerControl>();

            if (target != null)
            {
                foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
                {
                    if (deadBody.ParentId == target.ParentId) deadBody.gameObject.Destroy();
                }
            }

            foreach (var poisoner in Role.GetRoles(RoleEnum.Poisoner))
            {
                var poisonerRole = (Poisoner)poisoner;
                if (poisonerRole.PoisonedPlayer == player) poisonerRole.PoisonedPlayer = null;
            }

            player.Revive();
            Murder.KilledPlayers.Remove(
                Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == target.ParentId));
            revived.Add(player);
            var usedPosition = new Vector2(position.x, position.y + 0.3636f);
            player.transform.position = new Vector2(usedPosition.x, usedPosition.y);
            RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);

            if (PlayerControl.LocalPlayer == player) player.myTasks.RemoveAt(1);

            if (Patches.SubmergedCompatibility.isSubmerged() && PlayerControl.LocalPlayer.PlayerId == target.ParentId)
            {
                Patches.SubmergedCompatibility.ChangeFloor(target.transform.position.y > -7);
            }
            if (target != null) Object.Destroy(target.gameObject);

            if (player.Is(ModifierEnum.Celebrity))
            {
                var celeb = Modifier.GetModifier<Celebrity>(player);
                celeb.JustDied = false;
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
            Utils.TurnMadmate(player, false);
            return;
        }
    }
}