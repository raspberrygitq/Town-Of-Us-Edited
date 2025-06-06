namespace TownOfUsEdited.Roles
{
    public class Assassin : Role
    {
        public Assassin(PlayerControl player) : base(player)
        {
            Name = "Assassin";
            ImpostorText = () => "Kill Everyone During Meetings";
            TaskText = () => "Guess the roles of <color=#00FFFF>Crewmates</color> mid-meeting to kill them!\nFake Tasks:";
            Color = Patches.Colors.Impostor;
            RoleType = RoleEnum.Assassin;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            Alignment = Alignment.ImpostorKilling;
        }
    }
}
