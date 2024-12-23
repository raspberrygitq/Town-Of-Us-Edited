using System;
using System.Collections.Generic;
using TMPro;
using TownOfUs.CrewmateRoles.TrapperMod;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Trapper : Role
    {
        public static Material trapMaterial = TownOfUs.bundledAssets.Get<Material>("trap");

        public List<Trap> traps = new List<Trap>();
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public int UsesLeft;
        public TextMeshPro UsesText;

        public List<RoleEnum> trappedPlayers;

        public bool ButtonUsable => UsesLeft != 0;
        public Trapper(PlayerControl player) : base(player)
        {
            Name = "Trapper";
            ImpostorText = () => "Catch Killers In The Act";
            TaskText = () => "Place traps around the map";
            Color = Patches.Colors.Trapper;
            RoleType = RoleEnum.Trapper;
            Faction = Faction.Crewmates;
            Alignment = Alignment.CrewmateInvestigative;
            Cooldown = CustomGameOptions.TrapCooldown;
            trappedPlayers = new List<RoleEnum>();
            AddToRoleHistory(RoleType);

            UsesLeft = CustomGameOptions.MaxTraps;
        }

        public float TrapTimer()
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
