using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class TalkativeWolf : Role
    {
        private KillButton _rampageButton;
        public bool Rampaged = false;
        public float RampageCooldown;
        public bool RampagecoolingDown => RampageCooldown > 0f;
        public TalkativeWolf(PlayerControl player) : base(player)
        {
            Name = "Talkative Wolf";
            ImpostorText = () => "Yesterday, I went to the shop and...";
            TaskText = () => "Don't forget to say the word to save you\nFake Tasks:";
            Color = Patches.Colors.TalkativeWolf;
            RoleType = RoleEnum.TalkativeWolf;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            RampageCooldown = CustomGameOptions.RampageCD;
        }

        public KillButton RampageButton
        {
            get => _rampageButton;
            set
            {
                _rampageButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float RampageTimer()
        {
            if (!RampagecoolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                RampageCooldown -= Time.deltaTime;
                return RampageCooldown;
            }
            else return RampageCooldown;
        }
    }
}