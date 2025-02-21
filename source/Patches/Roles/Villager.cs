namespace TownOfUsEdited.Roles
{
    public class Villager : Role
    {
        public Villager(PlayerControl player) : base(player)
        {
            Name = "Villager";
            ImpostorText = () => "Find The <color=#A86629FF>Werewolves</color>";
            TaskText = () => "Find out the identity of the <color=#A86629FF>Werewolves</color>";
            Faction = Faction.Crewmates;
            RoleType = RoleEnum.Villager;
            AddToRoleHistory(RoleType);
            Color = Patches.Colors.Villager;
        }
    }
}