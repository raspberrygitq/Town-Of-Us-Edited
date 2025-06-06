using TMPro;

namespace TownOfUsEdited.Roles
{
    public class Shooter : Role

    {
        public KillButton _storeButton;
        public int UsesLeft;
        public TextMeshPro UsesText;
        public TextMeshPro StoreText;
        public PlayerControl ClosestPlayer;
        public bool ButtonUsable => UsesLeft != CustomGameOptions.MaxStore;
        public bool Killing = false;
        public Shooter(PlayerControl player) : base(player)
        {
            Name = "Shooter";
            ImpostorText = () => "Look At My Gun!";
            TaskText = () => "Store bullets to perform multiple kills\nFake Tasks:";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Shooter;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            Alignment = Alignment.ImpostorKilling;
        }
        public KillButton StoreButton
        {
            get => _storeButton;
            set
            {
                _storeButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public void Kill(PlayerControl target)
        {
            Utils.Interact(PlayerControl.LocalPlayer, target, true);
        }
    }
}