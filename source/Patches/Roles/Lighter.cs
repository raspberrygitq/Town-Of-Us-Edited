using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Lighter : Role
    {
        public Lighter(PlayerControl owner) : base(owner)
        {
            Name = "Lighter";
            ImpostorText = () => "Your Vision Is Powerful";
            TaskText = () => "Use your super vision to catch the <color=#FF0000FF>Impostors</color>";
            Color = Patches.Colors.Lighter;
            Cooldown = CustomGameOptions.LightCD;
            RoleType = RoleEnum.Lighter;
            Alignment = Alignment.CrewmateSupport;
            AddToRoleHistory(RoleType);
            Faction = Faction.Crewmates;
        }
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public float TimeRemaining;
        public bool Enabled;
        public bool UsingLight => TimeRemaining > 0f;
        public float LightCooldown()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }

        public void StartLight()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }
        
        public void StopLight()
        {
            Enabled = false;
            Cooldown = CustomGameOptions.LightCD;
        }
    }
}