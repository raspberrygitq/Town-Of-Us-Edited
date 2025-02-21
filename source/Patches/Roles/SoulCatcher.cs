namespace TownOfUsEdited.Roles
{
    public class SoulCatcher : Role
    {
        public SoulCatcher(PlayerControl player) : base(player)
        {
            Name = "Soul Catcher";
            ImpostorText = () => "Observe The Deads";
            TaskText = () => "Find informations by following the deads";
            Faction = Faction.Crewmates;
            RoleType = RoleEnum.SoulCatcher;
            AddToRoleHistory(RoleType);
            Color = Patches.Colors.SoulCatcher;
        }
    }
}