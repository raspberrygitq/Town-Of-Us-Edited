using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Helper : Role
    {
        public bool Enabled;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public float TimeRemaining;
        public PlayerControl ClosestPlayer;
        public PlayerControl AlertedPlayer;
        public bool OnAlert => TimeRemaining > 0f;

        public Helper(PlayerControl player) : base(player)
        {
            Name = "Helper";
            ImpostorText = () => "Easter Egg fr";
            TaskText = () => "Alert the remaining crew about an incoming danger";
            Color = Patches.Colors.Helper;
            RoleType = RoleEnum.Helper;
            AddToRoleHistory(RoleType);
            Cooldown = CustomGameOptions.HelperCD;
            AlertedPlayer = null;
            Alignment = Alignment.CrewmateGhost;
        }

        public float AlertTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }

        public void Alert()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }

        public void UnAlert()
        {
            Enabled = false;
            AlertedPlayer = null;
            Cooldown = CustomGameOptions.HelperCD;
        }
    }
}