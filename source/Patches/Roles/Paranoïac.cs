using System;
using TMPro;

namespace TownOfUsEdited.Roles
{
    public class Paranoïac : Role
    {
        public Paranoïac(PlayerControl player) : base(player)
        {
            Name = "Paranoïac";
            ImpostorText = () => "Impostor?? Please don't kill me!!";
            TaskText = () => "Hide in vents and start a Meeting if needed";
            Color = Patches.Colors.Paranoïac;
            RoleType = RoleEnum.Paranoïac;
            Faction = Faction.Crewmates;
            Alignment = Alignment.CrewmateSupport;
            AddToRoleHistory(RoleType);
            if (CustomGameOptions.GameMode != GameMode.Werewolf)
            {
                UsesLeft = CustomGameOptions.MaxMeetings;
            }
            else
            {
                UsesLeft = 1;
            }
        }
        public int UsesLeft;
        public TextMeshPro UsesText;
        public bool ButtonUsable => UsesLeft != 0;
    }
}