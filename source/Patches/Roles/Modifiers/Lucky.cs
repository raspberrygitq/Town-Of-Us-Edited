namespace TownOfUsEdited.Roles.Modifiers
{
    public class Lucky : Modifier
    {
        public Lucky(PlayerControl player) : base(player)
        {
            Name = "Lucky";
            TaskText = () => "Your Kill Cooldown is Random";
            Color = Patches.Colors.Impostor;
            ModifierType = ModifierEnum.Lucky;
        }
    }
}