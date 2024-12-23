using System.Linq;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Coven : Role
    {
        public Coven(PlayerControl owner) : base(owner)
        {
            Name = "Coven";
            ImpostorText = () => "Kill Everyone And Reign";
            TaskText = () => "Kill all non Coven members\nFake Tasks:";
            Color = Patches.Colors.Coven;
            RoleType = RoleEnum.Coven;
            AddToRoleHistory(RoleType);
            Faction = Faction.Coven;
            Cooldown = CustomGameOptions.CovenKCD;
        }

        public KillButton _sabotageButton;
        public PlayerControl ClosestPlayer;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
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
            var covenTeam = new List<PlayerControl>();
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