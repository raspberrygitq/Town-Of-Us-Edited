namespace TownOfUs.Roles
{
    public class Guard : Role
    {
        public PlayerControl ClosestPlayer;
        public PlayerControl ProtectedPlayer;
        public bool UsedProtect = false;
        public Guard(PlayerControl player) : base(player)
        {
            Name = "Guard";
            ImpostorText = () => "Protect the <color=#adf34b>Villagers</color>";
            TaskText = () => "Protect the <color=#adf34b>Villagers</color> with a shield";
            Faction = Faction.Crewmates;
            RoleType = RoleEnum.Guard;
            AddToRoleHistory(RoleType);
            Color = Patches.Colors.Guard;
        }
    }
}