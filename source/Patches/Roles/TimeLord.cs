using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class TimeLord : Role
    {
        public TimeLord(PlayerControl player) : base(player)
        {
            Name = "Time Lord";
            ImpostorText = () => "Control The Time";
            TaskText = () => "Rewind the time to save the crew";
            Color = Patches.Colors.TimeLord;
            Cooldown = CustomGameOptions.RewindCooldown;
            RoleType = RoleEnum.TimeLord;
            Alignment = Alignment.CrewmatePower;
            AddToRoleHistory(RoleType);
            Scale = 1.4f;
        }

        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public float TimeRemaining;
        public bool Rewinding = false;
        public bool UsingRewind => TimeRemaining > 0f;

        public float RewindTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }

        public void Rewind()
        {
            Rewinding = true;
            TimeRemaining -= Time.deltaTime;
        }


        public void StopRewind()
        {
            Rewinding = false;
            Cooldown = CustomGameOptions.RewindCooldown;
        }
    }
}