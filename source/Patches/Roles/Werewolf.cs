using System;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Werewolf : Role
    {
        private KillButton _rampageButton;
        public bool Rampaged = false;
        public float RampageCooldown;
        public bool RampagecoolingDown => RampageCooldown > 0f;
        public Werewolf(PlayerControl player) : base(player)
        {
            Name = "Werewolf";
            ImpostorText = () => "Kill The Villagers";
            TaskText = () => "Kill all <color=#adf34b>Villagers</color> without getting caught\nFake Tasks:";
            Color = Patches.Colors.Werewolf;
            RoleType = RoleEnum.Werewolf;
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
    }
}