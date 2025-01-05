using System.Linq;
using AmongUs.GameOptions;
using TownOfUs.Patches;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class BlackWolf : Role
    {
        private KillButton _rampageButton;
        private KillButton _convertButton;
        public PlayerControl ClosestPlayer;
        public bool Rampaged = false;
        public bool UsedConvert = false;
        public float RampageCooldown;
        public bool RampagecoolingDown => RampageCooldown > 0f;
        public BlackWolf(PlayerControl player) : base(player)
        {
            Name = "Black Wolf";
            ImpostorText = () => "Convert A Villager";
            TaskText = () => "Convert a <color=#adf34b>Villager</color> to your side\nFake Tasks:";
            Color = Patches.Colors.BlackWolf;
            RoleType = RoleEnum.BlackWolf;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            RampageCooldown = CustomGameOptions.RampageCD;
        }

        public KillButton RampageButton
        {
            get => _rampageButton;
            set
            {
                _rampageButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public KillButton ConvertButton
        {
            get => _convertButton;
            set
            {
                _convertButton = value;
                ExtraButtons.Add(value);
            }
        }

        public float RampageTimer()
        {
            if (!RampagecoolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                RampageCooldown -= Time.deltaTime;
                return RampageCooldown;
            }
            else return RampageCooldown;
        }

        public void ConvertAbility(PlayerControl target)
        {
            if (target.Is(Faction.Impostors) || target.Is(Faction.NeutralKilling) || target.Is(RoleEnum.Mayor)) return;

            UsedConvert = true;
            Convert(target);
            Utils.Rpc(CustomRPC.WerewolfConvert, PlayerControl.LocalPlayer.PlayerId, target.PlayerId);
        }

        public void Convert(PlayerControl newwolf)
        {
            var oldRole = Role.GetRole(newwolf);
            var killsList = (oldRole.CorrectKills, oldRole.IncorrectKills, oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
            Role.RoleDictionary.Remove(newwolf.PlayerId);

            if (oldRole.ExtraButtons.Any())
            {
                foreach (var button in oldRole.ExtraButtons)
                {
                    if (PlayerControl.LocalPlayer == newwolf)
                    {
                        button.gameObject.SetActive(false);
                    }
                }
            }

            if (PlayerControl.LocalPlayer == newwolf)
            {
                var role = new Werewolf(PlayerControl.LocalPlayer);
                role.CorrectKills = killsList.CorrectKills;
                role.IncorrectKills = killsList.IncorrectKills;
                role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                role.RegenTask();
            }
            else
            {
                var role = new Werewolf(newwolf);
                role.CorrectKills = killsList.CorrectKills;
                role.IncorrectKills = killsList.IncorrectKills;
                role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                role.RegenTask();
            }

            PlayerControl_Die.Postfix();
        }
    }
}