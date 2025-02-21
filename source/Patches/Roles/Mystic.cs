namespace TownOfUsEdited.Roles
{
    public class Mystic : Role
    {
        public Mystic(PlayerControl player) : base(player)
        {
            Name = "Mystic";
            ImpostorText = () => "Understand When Kills Happen";
            TaskText = () => "You will know whoever dies and whenever they die";
            Color = Patches.Colors.Mystic;
            RoleType = RoleEnum.Mystic;
            Faction = Faction.Crewmates;
            Alignment = Alignment.CrewmateInvestigative;
            AddToRoleHistory(RoleType);
        }
    }
}