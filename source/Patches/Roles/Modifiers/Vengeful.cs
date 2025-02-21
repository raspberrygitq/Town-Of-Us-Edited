namespace TownOfUsEdited.Roles.Modifiers
{
    public class Vengeful : Modifier
    {
        public Vengeful(PlayerControl player) : base(player)
        {
            Name = "Vengeful";
            TaskText = () => "You may kill someone after completing all your tasks";
            Color = Patches.Colors.Vengeful;
            ModifierType = ModifierEnum.Vengeful;
        }

        public bool UsedAbility = false;

        public KillButton KillButton;
        public PlayerControl ClosestPlayer;
    }
}