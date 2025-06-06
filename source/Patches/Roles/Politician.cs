using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TownOfUsEdited.Extensions;

namespace TownOfUsEdited.Roles
{
    public class Politician : Role
    {
        public PlayerControl ClosestPlayer;
        public List<byte> CampaignedPlayers = new List<byte>();
        public bool coolingDown => Cooldown > 0f;
        public float Cooldown;
        public bool CanCampaign;

        public Politician(PlayerControl player) : base(player)
        {
            Name = "Politician";
            ImpostorText = () => "Campaign To Become The Mayor!";
            TaskText = () => "Spread your campaign to become the Mayor!";
            Color = Patches.Colors.Politician;
            RoleType = RoleEnum.Politician;
            AddToRoleHistory(RoleType);
            CanCampaign = true;
            Alignment = Alignment.CrewmateSupport;
            Cooldown = CustomGameOptions.CampaignCd;
            Alignment = Alignment.CrewmatePower;
        }
        public GameObject RevealButton = new GameObject();

        public float CampaignTimer()
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