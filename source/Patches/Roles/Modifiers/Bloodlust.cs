namespace TownOfUs.Roles.Modifiers
{
    public class Bloodlust : Modifier
    {
        public int KilledThisRound;
        public Bloodlust(PlayerControl player) : base(player)
        {
            Name = "Bloodlust";
            TaskText = () => "You have a shorter kill cooldown after 2 kills";
            Color = Patches.Colors.Impostor;
            ModifierType = ModifierEnum.Bloodlust;
            KilledThisRound = 0;
        }
    }
}