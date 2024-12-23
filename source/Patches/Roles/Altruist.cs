namespace TownOfUs.Roles
{
    public class Altruist : Role
    {
        public DeadBody CurrentTarget;
        
        public Altruist(PlayerControl player) : base(player)
        {
            Name = "Altruist";
            ImpostorText = () => "Sacrifice Yourself To Save Another";
            TaskText = () => "Revive a dead body at the cost of your own life";
            Color = Patches.Colors.Altruist;
            RoleType = RoleEnum.Altruist;
            Faction = Faction.Crewmates;
            Alignment = Alignment.CrewmateProtective;
            AddToRoleHistory(RoleType);
        }
    }
}