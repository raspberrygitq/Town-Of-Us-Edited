using System.Collections.Generic;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Miner : Role
    {
        public readonly List<Vent> Vents = new List<Vent>();

        public KillButton _mineButton;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;


        public Miner(PlayerControl player) : base(player)
        {
            Name = "Miner";
            ImpostorText = () => "From The Top, Make It Drop, That's A Vent";
            TaskText = () => "Place vents around the map\nFake Tasks:";
            Color = Patches.Colors.Impostor;
            Cooldown = CustomGameOptions.MineCd;
            RoleType = RoleEnum.Miner;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            Alignment = Alignment.ImpostorSupport;
        }

        public bool CanPlace { get; set; }
        public Vector2 VentSize { get; set; }

        public KillButton MineButton
        {
            get => _mineButton;
            set
            {
                _mineButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float MineTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }
    }
}