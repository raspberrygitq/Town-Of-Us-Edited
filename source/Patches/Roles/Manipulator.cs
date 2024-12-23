namespace TownOfUs.Roles
{
    public class Manipulator : Role

    {
        public KillButton _manipulateButton;
        public PlayerControl ClosestPlayer;
        public PlayerControl ManipulatedPlayer;
        public bool IsManipulating = false;

        public Manipulator(PlayerControl player) : base(player)
        {
            Name = "Manipulator";
            ImpostorText = () => "Manipulate The Crewmates";
            TaskText = () => "Use your ability to force <color=#00FFFF>Crewmates</color> to kill each other\nFake Tasks:";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Manipulator;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            Alignment = Alignment.ImpostorKilling;
        }
        public KillButton ManipulateButton
        {
            get => _manipulateButton;
            set
            {
                _manipulateButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
    }
}