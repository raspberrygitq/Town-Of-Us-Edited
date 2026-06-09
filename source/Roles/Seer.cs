using System.Collections.Generic;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Seer : Role
    {
        public List<byte> Investigated = new List<byte>();
        public List<byte> Revealed = new List<byte>();

        public Seer(PlayerControl player) : base(player)
        {
            Name = "Seer";
            ImpostorText = () => "Reveal The Alliance Of Other Players";
            TaskText = () => "Reveal alliances of other players to find the <color=#FF0000>Impostors</color>";
            Color = Patches.Colors.Seer;
            Cooldown = CustomGameOptions.SeerCd;
            RoleType = RoleEnum.Seer;
            Faction = Faction.Crewmates;
            Alignment = Alignment.CrewmateInvestigative;
            AddToRoleHistory(RoleType);
        }

        public PlayerControl ClosestPlayer;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public bool UsedReveal = false;

        public float SeerTimer()
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