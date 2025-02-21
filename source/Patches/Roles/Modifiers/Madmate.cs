namespace TownOfUsEdited.Roles.Modifiers
{
    public class Madmate : Modifier
    {
        public Madmate(PlayerControl player) : base(player)
        {
            Name = "Madmate";
            TaskText = () => "You are now part of the Impostors!";
            Color = Palette.ImpostorRed;
            ModifierType = ModifierEnum.Madmate;
            Utils.TurnMadmate(Player, true);
        }

        public enum BecomeMadmateOptions
        {
            Die,
            OriginalFaction,
            Impostor
        }
    }
}