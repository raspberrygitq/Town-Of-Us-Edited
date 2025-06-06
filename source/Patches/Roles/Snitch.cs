using System.Collections.Generic;
using TownOfUsEdited.Extensions;
using UnityEngine;
using System.Linq;
using TownOfUsEdited.CrewmateRoles.ImitatorMod;

namespace TownOfUsEdited.Roles
{
    public class Snitch : Role
    {
        public List<ArrowBehaviour> ImpArrows = new List<ArrowBehaviour>();

        public Dictionary<byte, ArrowBehaviour> SnitchArrows = new Dictionary<byte, ArrowBehaviour>();

        public Snitch(PlayerControl player) : base(player)
        {
            Name = "Snitch";
            ImpostorText = () => "Complete All Your Tasks To Discover The <color=#FF0000>Impostors</color>";
            TaskText = () =>
                TasksDone
                    ? "Find the arrows pointing to the <color=#FF0000>Impostors</color>!"
                    : "Complete all your tasks to discover the <color=#FF0000>Impostors</color>!";
            Color = Patches.Colors.Snitch;
            RoleType = RoleEnum.Snitch;
            Faction = Faction.Crewmates;
            Alignment = Alignment.CrewmateInvestigative;
            AddToRoleHistory(RoleType);
        }

        public bool Revealed => TasksLeft <= CustomGameOptions.SnitchTasksRemaining;
        public bool TasksDone => TasksLeft <= 0;

        internal override bool Criteria()
        {
            return Revealed && PlayerControl.LocalPlayer.Data.IsImpostor() && !Player.Data.IsDead ||
                   base.Criteria();
        }

        internal override bool RoleCriteria()
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if ((localPlayer.Data.IsImpostor() || GetRole(localPlayer).Faction == Faction.NeutralKilling) && !Player.Data.IsDead)
            {
                return (Revealed && !StartImitate.ImitatingPlayers.Contains(Player.PlayerId)) || base.RoleCriteria();
            }
            return false || base.RoleCriteria();
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = SnitchArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
            SnitchArrows.Remove(arrow.Key);
        }
    }
}