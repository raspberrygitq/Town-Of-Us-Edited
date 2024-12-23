using System.Linq;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Bodyguard : Role
    {
        public bool Protected = false;
        public Bodyguard(PlayerControl player) : base(player)
        {
            Name = "Bodyguard";
            ImpostorText = () => "Protect The Crew";
            TaskText = () => "Protect crewmates against <color=#FF0000FF>Impostor</color> attacks";
            Faction = Faction.Crewmates;
            RoleType = RoleEnum.Bodyguard;
            Alignment = Alignment.CrewmateProtective;
            AddToRoleHistory(RoleType);
            Color = Patches.Colors.Bodyguard;
        }
    }
}