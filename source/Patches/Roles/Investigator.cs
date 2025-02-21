using System.Collections.Generic;
using TownOfUsEdited.CrewmateRoles.InvestigatorMod;

namespace TownOfUsEdited.Roles
{
    public class Investigator : Role
    {
        public readonly List<Footprint> AllPrints = new List<Footprint>();


        public Investigator(PlayerControl player) : base(player)
        {
            Name = "Investigator";
            ImpostorText = () => "Find All Impostors By Examining Footprints";
            TaskText = () => "You can see everyone's footprints";
            Color = Patches.Colors.Investigator;
            RoleType = RoleEnum.Investigator;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            Alignment = Alignment.CrewmateInvestigative;
            Scale = 1.4f;
        }
    }
}