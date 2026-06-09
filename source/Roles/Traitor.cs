using System.Collections.Generic;
using TMPro;

namespace TownOfUsEdited.Roles
{
    public class Traitor : Role
    {
        public List<RoleEnum> CanBeRoles = new List<RoleEnum>();
        public List<RoleEnum> SelectedRoles = new List<RoleEnum>();
        public KillButton _changeRoleButton;
        public TextMeshPro ChangeText;
        public Traitor(PlayerControl player) : base(player)
        {
            Name = "Traitor";
            ImpostorText = () => "";
            TaskText = () => "Betray the <color=#00FFFF>Crewmates</color>!\nFake Tasks:";
            Color = Patches.Colors.Impostor;
            RoleType = RoleEnum.Traitor;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            Alignment = Alignment.ImpostorKilling;
        }

        public KillButton ChangeRoleButton
        {
            get => _changeRoleButton;
            set
            {
                _changeRoleButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
    }
}