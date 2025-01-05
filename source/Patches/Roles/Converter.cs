using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using Reactor.Utilities.Extensions;
using TownOfUs.CrewmateRoles.MedicMod;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Converter : Role
    {
        public Converter(PlayerControl owner) : base(owner)
        {
            Name = "Converter";
            ImpostorText = () => "Convert someone to your side";
            TaskText = () => "Convert an innocent to help you killing\nFake Tasks:";
            Color = Patches.Colors.Impostor;
            Cooldown = CustomGameOptions.ConverterCD;
            RoleType = RoleEnum.Converter;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            Alignment = Alignment.ImpostorSupport;
        }

        public DeadBody CurrentTarget;
        public KillButton _convertButton;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
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
            Cooldown = CustomGameOptions.ConverterCD;
        }
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

        public static void Revive(DeadBody target)
        {
            var playerId = target.ParentId;
            var player = Utils.PlayerById(playerId);
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
                if (poisonerRole.PoisonedPlayer == player) poisonerRole.PoisonedPlayer = poisonerRole.Player;
            }

            player.Revive();
            Murder.KilledPlayers.Remove(
                Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == target.ParentId));
            revived.Add(player);
            player.NetTransform.SnapTo(new Vector2(position.x, position.y + 0.3636f));
            RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);

            if (PlayerControl.LocalPlayer == player) player.myTasks.RemoveAt(1);

            if (Patches.SubmergedCompatibility.isSubmerged() && PlayerControl.LocalPlayer.PlayerId == target.ParentId)
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
            Utils.TurnMadmate(player, false);
            return;
        }
    }
}