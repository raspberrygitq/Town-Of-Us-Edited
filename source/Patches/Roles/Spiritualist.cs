using System.Linq;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Spiritualist : Role
    {
        public Spiritualist(PlayerControl owner) : base(owner)
        {
            Name = "Spiritualist";
            ImpostorText = () => "Control Players";
            TaskText = () => "Control players to immunise the Coven\nFake Tasks:";
            Color = Patches.Colors.Coven;
            RoleType = RoleEnum.Spiritualist;
            AddToRoleHistory(RoleType);
            Faction = Faction.Coven;
        }

        public KillButton _controlButton;
        public PlayerControl ClosestPlayer;
        public PlayerControl ControlledPlayer;
        public void Control(PlayerControl target)
        {
            var interact = Utils.Interact(PlayerControl.LocalPlayer, target);
            if (interact[4] == true)
            {
                ControlledPlayer = target;
                KillCooldown = CustomGameOptions.CovenKCD;
                return;
            }
            if (interact[0] == true)
            {
                KillCooldown = CustomGameOptions.ProtectKCReset;
                return;
            }
            else if (interact[1] == true)
            {
                KillCooldown = CustomGameOptions.VestKCReset;
                return;
            }
            else if (interact[3] == true) return;
        }
        public KillButton ControlButton
        {
            get => _controlButton;
            set
            {
                _controlButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var covenTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            covenTeam.Add(PlayerControl.LocalPlayer);
            var toAdd = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Coven) && x != PlayerControl.LocalPlayer).ToList();
            foreach (var player in toAdd)
            {
                covenTeam.Add(player);
            }
            __instance.teamToShow = covenTeam;
        }
    }
}