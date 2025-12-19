using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Plumber : Role
    {
        public KillButton _flushButton; // No way tour uses the actual vent button :skull:
        public bool coolingDown => Cooldown > 0f;
        public float Cooldown;
        public Vent Vent;
        public List<byte> FutureBlocks = new List<byte>();
        public List<byte> VentsBlocked = new List<byte>();
        public List<GameObject> Block = new List<GameObject>();
        public TextMeshPro FlushText;
        public KillButton FlushButton
        {
            get => _flushButton;
            set
            {
                _flushButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public Plumber(PlayerControl player) : base(player)
        {
            Name = "Plumber";
            ImpostorText = () => "Get The Rats Out Of The Sewers";
            TaskText = () => "Maintain a clean vent system";
            Color = Patches.Colors.Plumber;
            RoleType = RoleEnum.Plumber;
            AddToRoleHistory(RoleType);
            UsesLeft = CustomGameOptions.MaxBarricades;
            Cooldown = CustomGameOptions.FlushCd;
            Alignment = Alignment.CrewmateSupport;
        }

        public int UsesLeft;
        public TextMeshPro UsesText;

        public bool ButtonUsable => UsesLeft != 0;

        public float FlushTimer()
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