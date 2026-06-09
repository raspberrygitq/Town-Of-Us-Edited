namespace TownOfUsEdited.Roles
{
    public class Impostor : Role
    {
        public Impostor(PlayerControl player) : base(player)
        {
            Name = "Impostor";
            ImpostorText = () => "Kill And Sabotage";
            TaskText = () => "Don't let the <color=#00FFFF>Crewmates</color> find out who you are!\nFake Tasks:";
            Color = Patches.Colors.Impostor;
            RoleType = RoleEnum.Impostor;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }
    }
}