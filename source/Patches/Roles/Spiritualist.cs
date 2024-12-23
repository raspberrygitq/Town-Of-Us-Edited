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
            Cooldown = CustomGameOptions.CovenKCD;
        }

        public KillButton _sabotageButton;
        public KillButton _controlButton;
        public PlayerControl ClosestPlayer;
        public PlayerControl ControlledPlayer;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public void Control(PlayerControl target)
        {
            // Check if the Hex Master can hex
            if (Cooldown > 0)
                return;

            if (target.Is(Faction.Coven))
                return;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, target);
            if (interact[4] == true)
            {
                ControlledPlayer = target;
                Cooldown = CustomGameOptions.CovenKCD;
                return;
            }
            if (interact[0] == true)
            {
                Cooldown = CustomGameOptions.ProtectKCReset;
                return;
            }
            else if (interact[1] == true)
            {
                Cooldown = CustomGameOptions.VestKCReset;
                return;
            }
            else if (interact[3] == true) return;
        }

        public void Kill(PlayerControl target)
        {
            // Check if the Coven can kill
            if (Cooldown > 0)
                return;

            if (target.Is(Faction.Coven))
                return;

            Utils.Interact(PlayerControl.LocalPlayer, target, true);

            // Set the last kill time
            Cooldown = CustomGameOptions.CovenKCD;
        }
        public KillButton SabotageButton
        {
            get => _sabotageButton;
            set
            {
                _sabotageButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
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
        public float KillTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
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