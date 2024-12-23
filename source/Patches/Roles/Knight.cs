using TMPro;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Knight : Role
    {
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;

        public int UsesLeft;
        public TextMeshPro UsesText;
        public PlayerControl ClosestPlayer;

        public bool ButtonUsable => UsesLeft != 0;

        public Knight(PlayerControl player) : base(player)
        {
            Name = "Knight";
            ImpostorText = () => "Kill once, choose wisely";
            TaskText = () => "You can kill anyone but only once";
            Color = Patches.Colors.Knight;
            Cooldown = CustomGameOptions.KnightKCD;
            Faction = Faction.Crewmates;
            RoleType = RoleEnum.Knight;
            AddToRoleHistory(RoleType);
            Alignment = Alignment.CrewmateKilling;

            UsesLeft = 1;
        }

        public float KillTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }

        public void Kill(PlayerControl target)
        {
            if (Cooldown > 0)
                return;

            Utils.Interact(PlayerControl.LocalPlayer, target, true);

            Cooldown = CustomGameOptions.KnightKCD;

            UsesLeft--;
        }
    }
}