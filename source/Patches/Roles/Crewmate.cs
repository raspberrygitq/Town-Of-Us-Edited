namespace TownOfUs.Roles
{
    public class Crewmate : Role
    {
        public Crewmate(PlayerControl player) : base(player)
        {
            Name = "Crewmate";
            ImpostorText = () => "Do your tasks";
            TaskText = () => "Do your tasks and eject the <color=#FF0000FF>Impostors</color>";
            Faction = Faction.Crewmates;
            RoleType = RoleEnum.Crewmate;
            Alignment = Alignment.CrewmateInvestigative;
            AddToRoleHistory(RoleType);
            Color = Patches.Colors.Crewmate;
        }
    }
}