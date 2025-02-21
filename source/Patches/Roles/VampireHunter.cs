using System;
using TMPro;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class VampireHunter : Role
    {
        public PlayerControl ClosestPlayer;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;

        public int UsesLeft;
        public TextMeshPro UsesText;
        public bool AddedStakes;

        public bool ButtonUsable => UsesLeft != 0;

        public VampireHunter(PlayerControl player) : base(player)
        {
            Name = "Vampire Hunter";
            ImpostorText = () => "Stake The <color=#262626FF>Vampires</color>";
            TaskText = () => "Stake the <color=#262626FF>Vampires</color>";
            Color = Patches.Colors.VampireHunter;
            Cooldown = CustomGameOptions.StakeCd;
            RoleType = RoleEnum.VampireHunter;
            Faction = Faction.Crewmates;
            Alignment = Alignment.CrewmateKilling;
            AddToRoleHistory(RoleType);

            UsesLeft = 0;
            AddedStakes = false;
        }

        public float StakeTimer()
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