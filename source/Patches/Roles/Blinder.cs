using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Blinder : Role
    {
        public KillButton _blindButton;
        public RoleEnum formerRole = new RoleEnum();
        public PlayerControl ClosestPlayer;
        public PlayerControl BlindedPlayer;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public bool Enabled;
        public float TimeRemaining;
        public bool Blinding => TimeRemaining > 0f;

        public Blinder(PlayerControl player) : base(player)
        {
            Name = "Blinder";
            ImpostorText = () => "";
            TaskText = () => "Blind <color=#00FFFF>Crewmates</color> to help your teamates!\nFake Tasks:";
            Color = Patches.Colors.Impostor;
            Cooldown = CustomGameOptions.BlindCD;
            RoleType = RoleEnum.Blinder;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            Alignment = Alignment.ImpostorGhost;
        }
        public KillButton BlindButton
        {
            get => _blindButton;
            set
            {
                _blindButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float BlindTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }

        public void Blind()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }

        public void UnBlind()
        {
            Enabled = false;
            BlindedPlayer = null;
            Cooldown = CustomGameOptions.BlindCD;
        }
    }
}