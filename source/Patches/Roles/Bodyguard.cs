using System.Linq;
using TownOfUsEdited.Extensions;

namespace TownOfUsEdited.Roles
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