using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Watcher : Role
    {
        public PlayerControl ClosestPlayer;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;

        public int UsesLeft;
        public TextMeshPro UsesText;

        public bool ButtonUsable => UsesLeft != 0;
        public Dictionary<byte, List<RoleEnum>> Watching { get; set; } = new();

        public Watcher(PlayerControl player) : base(player)
        {
            Name = "Watcher";
            ImpostorText = () => "Keep Your Eyes Wide Open";
            TaskText = () => "Watch other Crewmates";
            Color = Patches.Colors.Lookout;
            Cooldown = CustomGameOptions.WatcherCooldown;
            RoleType = RoleEnum.Watcher;
            AddToRoleHistory(RoleType);
            Alignment = Alignment.CrewmateInvestigative;

            UsesLeft = CustomGameOptions.MaxWatches;
        }

        public float WatchTimer()
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