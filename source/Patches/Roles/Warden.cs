using System;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Warden : Role
    {
        public PlayerControl ClosestPlayer;
        public PlayerControl Fortified;

        public Warden(PlayerControl player) : base(player)
        {
            Name = "Warden";
            ImpostorText = () => "Fortify Crewmates";
            TaskText = () => "Fortify the Crewmates";
            Color = Patches.Colors.Warden;
            RoleType = RoleEnum.Warden;
            AddToRoleHistory(RoleType);
            Alignment = Alignment.CrewmateProtective;
        }
    }
}