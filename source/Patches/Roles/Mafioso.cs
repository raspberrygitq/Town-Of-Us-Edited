namespace TownOfUs.Roles
{
    public class Mafioso : Role
    {
        public Mafioso(PlayerControl player) : base(player)
        {
            Name = "Mafioso";
            ImpostorText = () => "Unleash Yourself";
            TaskText = () => "Unleash your powers when all your teammates are dead\nFake Tasks:";
            Color = Patches.Colors.Impostor;
            RoleType = RoleEnum.Mafioso;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            Alignment = Alignment.ImpostorSupport;
        }
    }
}