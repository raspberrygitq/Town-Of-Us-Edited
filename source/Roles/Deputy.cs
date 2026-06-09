using System.Collections.Generic;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Deputy : Role
    {
        public PlayerControl ClosestPlayer;
        public PlayerControl Camping = null;
        public PlayerControl Killer = null;
        public bool CampedThisRound = false;
        public Dictionary<byte, GameObject> Buttons { get; set; } = new();

        public Deputy(PlayerControl player) : base(player)
        {
            Name = "Deputy";
            ImpostorText = () => "Camp Crewmates To Catch Their Killer";
            TaskText = () => "Camp crewmates then shoot their killer";
            Color = Patches.Colors.Deputy;
            RoleType = RoleEnum.Deputy;
            Alignment = Alignment.CrewmateKilling;
            AddToRoleHistory(RoleType);
        }
    }
}