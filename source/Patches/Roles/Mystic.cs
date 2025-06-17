using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace TownOfUsEdited.Roles
{
    public class Mystic : Role
    {
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new Dictionary<byte, ArrowBehaviour>();
        public Mystic(PlayerControl player) : base(player)
        {
            Name = "Mystic";
            ImpostorText = () => "Understand When Kills Happen";
            TaskText = () => "You will know whoever dies and whenever they die";
            Color = Patches.Colors.Mystic;
            RoleType = RoleEnum.Mystic;
            Faction = Faction.Crewmates;
            Alignment = Alignment.CrewmateInvestigative;
            AddToRoleHistory(RoleType);
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
            BodyArrows.Remove(arrow.Key);
        }
    }
}