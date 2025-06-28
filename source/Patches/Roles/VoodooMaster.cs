using System.Linq;
using TMPro;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class VoodooMaster : Role
    {
        public VoodooMaster(PlayerControl owner) : base(owner)
        {
            Name = "Voodoo Master";
            ImpostorText = () => "Use Voodoo Magic On Players";
            TaskText = () => "Use your Voodoo powers to get the vote control\nFake Tasks:";
            Color = Patches.Colors.Coven;
            RoleType = RoleEnum.VoodooMaster;
            AddToRoleHistory(RoleType);
            Faction = Faction.Coven;
            Cooldown = CustomGameOptions.CovenKCD;
        }

        public KillButton _voodooButton;
        public PlayerControl VoodooPlayer;
        public PlayerControl ClosestPlayer;
        public TextMeshPro VoodooText;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public bool Voted;
        public byte VotedFor;
        public KillButton VoodooButton
        {
            get => _voodooButton;
            set
            {
                _voodooButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float VoodooTimer()
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