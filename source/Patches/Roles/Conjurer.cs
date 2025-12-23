using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Conjurer : Role
    {
        public KillButton _curseButton;
        public PlayerControl CursedPlayer;
        public PlayerControl ClosestPlayer;
        public Sprite Kill;

        public Conjurer(PlayerControl player) : base(player)
        {
            Name = "Conjurer";
            ImpostorText = () => "Curse Players To Death";
            TaskText = () => "Curse players to force them to kill each other\nFake Tasks:";
            Color = Patches.Colors.Impostor;
            RoleType = RoleEnum.Conjurer;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            Alignment = Alignment.ImpostorKilling;
            Kill = HudManager.Instance.KillButton.graphic.sprite;
        }

        public KillButton CurseButton
        {
            get => _curseButton;
            set
            {
                _curseButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
    }
}