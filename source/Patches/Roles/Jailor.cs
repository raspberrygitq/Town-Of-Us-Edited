using System;
using System.Linq;
using TownOfUsEdited.Extensions;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Jailor : Role
    {
        public Jailor(PlayerControl owner) : base(owner)
        {
            Name = "Jailor";
            ImpostorText = () => "Execute order 66";
            TaskText = () => "Execute the <color=#FF0000FF>Impostors</color>";
            Color = Patches.Colors.Jailor;
            Cooldown = CustomGameOptions.JailCD;
            RoleType = RoleEnum.Jailor;
            AddToRoleHistory(RoleType);
            Faction = Faction.Crewmates;
            Alignment = Alignment.CrewmateKilling;
        }
        public PlayerVoteArea Jailed { get; set; }
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public bool CanJail = true;
        public bool JailedAssassin = false;
        public PlayerControl ClosestPlayer;
        public PlayerControl JailedPlayer;
        public float JailTimer()
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