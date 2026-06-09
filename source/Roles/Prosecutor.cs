namespace TownOfUsEdited.Roles
{
    public class Prosecutor : Role
    {
        public Prosecutor(PlayerControl player) : base(player)
        {
            Name = "Prosecutor";
            ImpostorText = () => "Exile One Person Of Your Choosing";
            TaskText = () => "Choose to exile anyone you want";
            Color = Patches.Colors.Prosecutor;
            RoleType = RoleEnum.Prosecutor;
            AddToRoleHistory(RoleType);
            Faction = Faction.Crewmates;
            Alignment = Alignment.CrewmatePower;
            StartProsecute = false;
            Prosecuted = false;
            ProsecuteThisMeeting = false;
            MaxProsecutes = CustomGameOptions.MaxProsecutes;
            ProsecutesRemaining = MaxProsecutes;
        }
        public bool ProsecuteThisMeeting { get; set; }
        public bool Prosecuted { get; set; }
        public bool StartProsecute { get; set; }
        public int MaxProsecutes { get; set; }
        public int ProsecutesRemaining { get; set; }
        public PlayerControl ProsecutedPlayer;
        public PlayerVoteArea Prosecute { get; set; }
    }
}
