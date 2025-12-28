using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Sheriff : Role
    {
        public Sheriff(PlayerControl player) : base(player)
        {
            Name = "Sheriff";
            ImpostorText = () => "Shoot The <color=#FF0000FF>Impostor</color>";
            TaskText = () => "Shoot the <color=#FF0000>Impostors</color> but don't kill <color=#00FFFF>Crewmates</color>";
            Color = Patches.Colors.Sheriff;
            Cooldown = CustomGameOptions.SheriffKillCd;
            RoleType = RoleEnum.Sheriff;
            Faction = Faction.Crewmates;
            Alignment = Alignment.CrewmateKilling;
            AddToRoleHistory(RoleType);
        }

        public PlayerControl ClosestPlayer;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;

        public float SheriffKillTimer()
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