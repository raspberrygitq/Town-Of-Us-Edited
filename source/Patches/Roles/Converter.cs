using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using Reactor.Utilities.Extensions;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using TownOfUs.Modifiers.UnderdogMod;

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
            RoleType = RoleEnum.Converter;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            Alignment = Alignment.ImpostorSupport;
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
            if (PlayerControl.LocalPlayer.Is(ModifierEnum.Underdog))
            {
                var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.DetonateDelay;
                var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = PerformKill.LastImp() ? lowerKC : (PerformKill.IncreasedKC() ? normalKC : upperKC);
            }
            else if (PlayerControl.LocalPlayer.Is(ModifierEnum.Lucky))
            {
                var num = Random.RandomRange(1f, 60f);
                Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = num;
            }
            else if (PlayerControl.LocalPlayer.Is(ModifierEnum.Bloodlust))
            {
                var modifier = Modifier.GetModifier<Bloodlust>(PlayerControl.LocalPlayer);
                var num = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown / 2;
                if (modifier.KilledThisRound >= 2) Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = num;
                else Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
            }
            else Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
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