namespace TownOfUs.Roles.Modifiers
{
    public class Spotter : Modifier
    {
        public Spotter(PlayerControl player) : base(player)
        {
            Name = "Spotter";
            TaskText = () => "You see votes color";
            Color = Patches.Colors.Spotter;
            ModifierType = ModifierEnum.Spotter;
        }
    }
}