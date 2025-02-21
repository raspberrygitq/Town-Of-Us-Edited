namespace TownOfUsEdited.Roles
{
    public class Superstar : Role
    {
        public bool Reported = false;
        public Superstar(PlayerControl player) : base(player)
        {
            Name = "Superstar";
            ImpostorText = () => "Alert Everyone When Dying";
            TaskText = () => "Alert all players whenever you die";
            Faction = Faction.Crewmates;
            RoleType = RoleEnum.Superstar;
            Alignment = Alignment.CrewmateSupport;
            AddToRoleHistory(RoleType);
            Color = Patches.Colors.Superstar;
        }
    }
}