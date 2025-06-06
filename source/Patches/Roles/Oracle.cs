using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Oracle : Role
    {
        public PlayerControl ClosestPlayer;
        public PlayerControl Blessed;
        public float Accuracy;
        public Faction RevealedFaction;
        public bool SavedBlessed;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public KillButton _blessButton;
        public float BlessCooldown;
        public bool blessCoolingDown => BlessCooldown > 0f;

        public PlayerControl ClosestBlessedPlayer;
        public PlayerControl Confessor;

        public Oracle(PlayerControl player) : base(player)
        {
            Name = "Oracle";
            ImpostorText = () => "Get Other Players To Confess Their Sins";
            TaskText = () => "Get another player to confess on your passing";
            Color = Patches.Colors.Oracle;
            Cooldown = CustomGameOptions.ConfessCd;
            BlessCooldown = CustomGameOptions.BlessCD;
            Accuracy = CustomGameOptions.RevealAccuracy;
            RoleType = RoleEnum.Oracle;
            Faction = Faction.Crewmates;
            Alignment = Alignment.CrewmateProtective;
            AddToRoleHistory(RoleType);
        }
        public float ConfessTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }
        public KillButton BlessButton
        {
            get => _blessButton;
            set
            {
                _blessButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float BlessTimer()
        {
            if (!blessCoolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                BlessCooldown -= Time.deltaTime;
                return BlessCooldown;
            }
            else return BlessCooldown;
        }
    }
}