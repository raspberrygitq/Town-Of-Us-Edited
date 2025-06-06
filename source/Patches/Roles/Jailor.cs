using TMPro;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Jailor : Role
    {
        public Jailor(PlayerControl owner) : base(owner)
        {
            Name = "Jailor";
            ImpostorText = () => "Execute order 66";
            TaskText = () => "Jail and execute the <color=#FF0000FF>Impostors</color>";
            Color = Patches.Colors.Jailor;
            Cooldown = CustomGameOptions.JailCD;
            RoleType = RoleEnum.Jailor;
            AddToRoleHistory(RoleType);
            Faction = Faction.Crewmates;
            Alignment = Alignment.CrewmatePower;
        }
        public PlayerVoteArea Jailed { get; set; }
        public GameObject JailCell = new GameObject();
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public bool CanJail = true;
        public PlayerControl ClosestPlayer;
        public PlayerControl JailedPlayer;
        public KillButton _releaseButton;
        public TextMeshPro ReleaseText;
        public KillButton ReleaseButton
        {
            get => _releaseButton;
            set
            {
                _releaseButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float JailTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }
    }
}