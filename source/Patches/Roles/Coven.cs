using Il2CppSystem.Collections.Generic;
using System.Linq;

namespace TownOfUsEdited.Roles
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