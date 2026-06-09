using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Guardian : Role
    {
        public bool Enabled;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public float TimeRemaining;
        public PlayerControl ClosestPlayer;
        public PlayerControl ProtectedPlayer;
        public bool Guarding => TimeRemaining > 0f;

        public Guardian(PlayerControl player) : base(player)
        {
            Name = "Guardian";
            ImpostorText = () => "";
            TaskText = () => "Save <color=#00FFFF>Crewmates</color> from Murder Attempts!";
            Color = Patches.Colors.Guardian;
            RoleType = RoleEnum.Guardian;
            AddToRoleHistory(RoleType);
            Cooldown = CustomGameOptions.GuardCD;
            ProtectedPlayer = null;
            Alignment = Alignment.CrewmateGhost;
        }

        public float GuardTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }

        public void Guard()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }

        public void UnGuard()
        {
            Enabled = false;
            ProtectedPlayer = null;
            Cooldown = CustomGameOptions.GuardCD;
        }
    }
}