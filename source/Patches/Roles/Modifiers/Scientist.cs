namespace TownOfUsEdited.Roles.Modifiers
{
    public class Scientist : Modifier
    {
        public Scientist(PlayerControl player) : base(player)
        {
            Name = "Scientist";
            TaskText = () => "You can see player's death reasons";
            Color = Patches.Colors.Scientist;
            ModifierType = ModifierEnum.Scientist;
        }
    }
}