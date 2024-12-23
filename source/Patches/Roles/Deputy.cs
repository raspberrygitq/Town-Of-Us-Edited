using System.Linq;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Deputy : Role
    {
        public Deputy(PlayerControl player) : base(player)
        {
            Name = "Deputy";
            ImpostorText = () => "One Shot, Whatever It Takes";
            TaskText = () => "Shoot one player, choose carefully";
            Color = Patches.Colors.Deputy;
            RoleType = RoleEnum.Deputy;
            AddToRoleHistory(RoleType);
            Faction = Faction.Crewmates;
            Alignment = Alignment.CrewmateKilling;
            StartShooting = false;
            UsedShoot = false;
        }
        public bool UsedShoot { get; set; }
        public bool StartShooting { get; set; }
        public PlayerVoteArea Shoot { get; set; }
    }
}
