using TMPro;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Noclip : Role
    {
        private KillButton _noclipButton;
        public TextMeshPro NoclipText;
        public bool Enabled;
        public Vector3 NoclipSafePoint = new();
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public float TimeRemaining;

        public Noclip(PlayerControl player) : base(player)
        {
            Name = "Noclip";
            ImpostorText = () => "Walk Through Walls";
            TaskText = () => "Walk through walls and surprise your enemies\nFake Tasks:";
            Color = Patches.Colors.Impostor;
            Cooldown = CustomGameOptions.NoclipCooldown;
            RoleType = RoleEnum.Noclip;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            Alignment = Alignment.ImpostorConcealing;
        }

        public bool Noclipped => TimeRemaining > 0f;

        public KillButton NoclipButton
        {
            get => _noclipButton;
            set
            {
                _noclipButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float NoclipTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }

        public void WallWalk()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            Player.Collider.enabled = false;
            if (Player.Data.IsDead)
            {
                TimeRemaining = 0f;
            }
        }

        public void UnWallWalk()
        {
            Enabled = false;
            Cooldown = CustomGameOptions.NoclipCooldown;
            Player.Collider.enabled = true;
        }
    }
}