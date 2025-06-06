
using TMPro;

namespace TownOfUsEdited.Roles
{
    public class Avenger : Role
    {
        public Avenger(PlayerControl player) : base(player)
        {
            Name = "Avenger";
            ImpostorText = () => "Avenge The <color=#00FFFF>Crewmates</color>";
            TaskText = () => "Avenge a dead player by killing his killer";
            Color = Patches.Colors.Avenger;
            RoleType = RoleEnum.Avenger;
            Faction = Faction.Crewmates;
            Alignment = Alignment.CrewmateKilling;
            AddToRoleHistory(RoleType);
        }

        public bool Avenging = false;
        private KillButton _avengeButton;
        public TextMeshPro AvengeText;
        public PlayerControl ClosestPlayer;
        public PlayerControl killer;
        public DeadBody CurrentTarget;
        public KillButton AvengeButton
        {
            get => _avengeButton;
            set
            {
                _avengeButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
    }
}