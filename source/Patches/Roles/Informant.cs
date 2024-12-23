namespace TownOfUs.Roles
{
    public class Informant : Role
    {
        public KillButton _vitalsButton;

        public Informant(PlayerControl player) : base(player)
        {
            Name = "Informant";
            ImpostorText = () => "Get informations from anywhere";
            TaskText = () => "Acces Admin and Vitals anywhere on the map";
            Color = Patches.Colors.Informant;
            RoleType = RoleEnum.Informant;
            Alignment = Alignment.CrewmateInvestigative;
            AddToRoleHistory(RoleType);
        }

        public KillButton VitalsButton
        {
            get => _vitalsButton;
            set
            {
                _vitalsButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
    }
}