using UnityEngine;
using TMPro;

namespace TownOfUsEdited.Roles
{
    public class Veteran : Role
    {
        public bool Enabled;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public float TimeRemaining;

        public int UsesLeft;
        public TextMeshPro UsesText;

        public bool ButtonUsable => UsesLeft != 0;

        public Veteran(PlayerControl player) : base(player)
        {
            Name = "Veteran";
            ImpostorText = () => "Alert To Kill Anyone Who Interacts With You";
            TaskText = () => "Alert to kill whoever interacts with you";
            Color = Patches.Colors.Veteran;
            Cooldown = CustomGameOptions.AlertCd;
            RoleType = RoleEnum.Veteran;
            Faction = Faction.Crewmates;
            Alignment = Alignment.CrewmateKilling;
            AddToRoleHistory(RoleType);

            UsesLeft = CustomGameOptions.MaxAlerts;
        }

        public bool OnAlert => TimeRemaining > 0f;

        public float AlertTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }

        public void Alert()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }


        public void UnAlert()
        {
            Enabled = false;
            Cooldown = CustomGameOptions.AlertCd;
        }
    }
}