using System;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Warden : Role
    {
        public PlayerControl ClosestPlayer;
        public PlayerControl Fortified;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;

        public Warden(PlayerControl player) : base(player)
        {
            Name = "Warden";
            ImpostorText = () => "Fortify Crewmates";
            TaskText = () => "Fortify the Crewmates";
            Color = Patches.Colors.Warden;
            Cooldown = CustomGameOptions.FortifyCd;
            RoleType = RoleEnum.Warden;
            AddToRoleHistory(RoleType);
            Alignment = Alignment.CrewmateProtective;
            Cooldown = CustomGameOptions.FortifyCd;
        }
        public float FortifyTimer()
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