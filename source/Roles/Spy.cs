namespace TownOfUsEdited.Roles
{
    public class Spy : Role
    {
        public Spy(PlayerControl player) : base(player)
        {
            Name = "Spy";
            ImpostorText = () => "Snoop Around And Find Stuff Out";
            TaskText = () => "Gain extra information on the Admin Table";
            Color = Patches.Colors.Spy;
            RoleType = RoleEnum.Spy;
            Faction = Faction.Crewmates;
            Alignment = Alignment.CrewmateInvestigative;
            AddToRoleHistory(RoleType);
        }
        KillButton _adminButton;

        public KillButton AdminButton
        {
            get => _adminButton;
            set
            {
                _adminButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
    }
}