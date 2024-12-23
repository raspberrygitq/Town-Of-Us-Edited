using System.Linq;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Prosecutor : Role
    {
        public Prosecutor(PlayerControl player) : base(player)
        {
            Name = "Prosecutor";
            ImpostorText = () => "Exile One Person Of Your Choosing";
            TaskText = () => "Choose to exile anyone you want";
            Color = Patches.Colors.Prosecutor;
            RoleType = RoleEnum.Prosecutor;
            AddToRoleHistory(RoleType);
            Faction = Faction.Crewmates;
            Alignment = Alignment.CrewmateSupport;
            StartProsecute = false;
            Prosecuted = false;
            ProsecuteThisMeeting = false;
        }
        public bool ProsecuteThisMeeting { get; set; }
        public bool Prosecuted { get; set; }
        public bool StartProsecute { get; set; }
        public PlayerControl ProsecutedPlayer;
        public PlayerVoteArea Prosecute { get; set; }
    }
}
