using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Mayor : Role
    {
        public Mayor(PlayerControl player) : base(player)
        {
            Name = "Mayor";
            ImpostorText = () => "Reveal Yourself To Save The Town";
            TaskText = () => "Lead the town to victory";
            Color = Patches.Colors.Mayor;
            RoleType = RoleEnum.Mayor;
            AddToRoleHistory(RoleType);
            Revealed = false;
            Alignment = Alignment.CrewmatePower;
        }
        public bool Revealed { get; set; }

        public GameObject RevealButton = new GameObject();

        internal override bool Criteria()
        {
            return Revealed && !Player.Data.IsDead || base.Criteria();
        }

        internal override bool RoleCriteria()
        {
            if (!Player.Data.IsDead) return Revealed || base.RoleCriteria();
            return false || base.RoleCriteria();
        }
    }
}