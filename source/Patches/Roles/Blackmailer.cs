using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Blackmailer : Role
    {
        public KillButton _blackmailButton;

        public PlayerControl ClosestPlayer;
        public PlayerControl Blackmailed;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public bool ShookAlready = false;

        public Blackmailer(PlayerControl player) : base(player)
        {
            Name = "Blackmailer";
            ImpostorText = () => "Silence Crewmates During Meetings";
            TaskText = () => "Silence a <color=#00FFFF>Crewmate</color> for the next meeting\nFake Tasks:";
            Color = Patches.Colors.Impostor;
            Cooldown = CustomGameOptions.BlackmailCd;
            RoleType = RoleEnum.Blackmailer;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            Alignment = Alignment.ImpostorSupport;
        }

        public KillButton BlackmailButton
        {
            get => _blackmailButton;
            set
            {
                _blackmailButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float BlackmailTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }

        public bool CanSeeBlackmailed(byte playerId)
        {
            return !CustomGameOptions.BlackmailInvisible || Blackmailed?.PlayerId == playerId || Player.PlayerId == playerId || Utils.PlayerById(playerId).Data.IsDead;
        }
    }
}