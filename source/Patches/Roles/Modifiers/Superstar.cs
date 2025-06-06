namespace TownOfUsEdited.Roles.Modifiers
{
    public class Superstar : Modifier
    {
        public bool Reported = false;
        public Superstar(PlayerControl player) : base(player)
        {
            Name = "Superstar";
            TaskText = () => "Alert all players whenever you die";
            ModifierType = ModifierEnum.Superstar;
            Color = Patches.Colors.Superstar;
        }
    }
}