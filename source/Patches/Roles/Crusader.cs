using System;
using System.Linq;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Crusader : Role
    {
        public Crusader(PlayerControl owner) : base(owner)
        {
            Name = "Crusader";
            ImpostorText = () => "Trick Killers";
            TaskText = () => "Trick the <color=#FF0000FF>Impostors</color>";
            Color = Patches.Colors.Crusader;
            Cooldown = CustomGameOptions.CrusadeCD;
            RoleType = RoleEnum.Crusader;
            AddToRoleHistory(RoleType);
            Faction = Faction.Crewmates;
            Alignment = Alignment.CrewmateProtective;
        }
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public PlayerControl ClosestPlayer;
        public PlayerControl CrusadedPlayer;
        public float CrusadeTimer()
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