using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Freezer : Role
    {
        public KillButton _freezeButton;
        public PlayerControl ClosestPlayer;
        public PlayerControl FrozenPlayer;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public bool Enabled;
        public float TimeRemaining;
        public bool Freezing => TimeRemaining > 0f;

        public Freezer(PlayerControl player) : base(player)
        {
            Name = "Freezer";
            ImpostorText = () => "";
            TaskText = () => "Freeze <color=#00FFFF>Crewmates</color> to help your teamates!\nFake Tasks:";
            Color = Patches.Colors.Impostor;
            Cooldown = CustomGameOptions.FreezeCD;
            RoleType = RoleEnum.Freezer;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            Alignment = Alignment.ImpostorGhost;
        }
        public KillButton FreezeButton
        {
            get => _freezeButton;
            set
            {
                _freezeButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float FreezeTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }

        public void Freeze()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }

        public void UnFreeze()
        {
            if (!Utils.Rewinding())
            {
                Utils.Rpc(CustomRPC.UnFreeze, FrozenPlayer.PlayerId);
                FrozenPlayer.moveable = true;
            }
            Enabled = false;
            FrozenPlayer = null;
            Cooldown = CustomGameOptions.FreezeCD;
        }
    }
}