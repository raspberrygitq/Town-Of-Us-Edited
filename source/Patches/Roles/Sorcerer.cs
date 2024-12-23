using System.Linq;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Sorcerer : Role
    {
        public PlayerControl ClosestPlayer;
        public PlayerControl PoisonedPlayer;
        public DeadBody CurrentTarget;
        public KillButton _reviveButton;
        public bool UsedPoison = false;
        public bool UsedRevive = false;
        public Sorcerer(PlayerControl player) : base(player)
        {
            Name = "Sorcerer";
            ImpostorText = () => "Poison the <color=#A86629FF>Werewolves</color>";
            TaskText = () => "Poison the <color=#A86629FF>Werewolves</color> and save the <color=#adf34b>Villagers</color>";
            Faction = Faction.Crewmates;
            RoleType = RoleEnum.Sorcerer;
            AddToRoleHistory(RoleType);
            Color = Patches.Colors.Sorcerer;
        }

        public KillButton ReviveButton
        {
            get => _reviveButton;
            set
            {
                _reviveButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
    }
}