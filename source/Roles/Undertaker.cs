using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Undertaker : Role
    {
        public KillButton _dragDropButton;

        public Undertaker(PlayerControl player) : base(player)
        {
            Name = "Undertaker";
            ImpostorText = () => "Drag Bodies And Hide Them";
            if (CustomGameOptions.GameMode == GameMode.Chaos)
            {
                TaskText = () => "Drag bodies around to hide them from being revived\nFake Tasks:";
            }
            else
            {
                TaskText = () => "Drag bodies around to hide them from being reported\nFake Tasks:";
            }
            Color = Patches.Colors.Impostor;
            Cooldown = CustomGameOptions.DragCd;
            RoleType = RoleEnum.Undertaker;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            Alignment = Alignment.ImpostorSupport;
        }

        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public DeadBody CurrentTarget { get; set; }
        public DeadBody CurrentlyDragging { get; set; }

        public KillButton DragDropButton
        {
            get => _dragDropButton;
            set
            {
                _dragDropButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float DragTimer()
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