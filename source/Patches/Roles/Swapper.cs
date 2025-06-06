using System.Collections.Generic;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Swapper : Role
    {
        public readonly List<GameObject> Buttons = new List<GameObject>();

        public readonly List<(byte, bool)> ListOfActives = new List<(byte, bool)>();


        public Swapper(PlayerControl player) : base(player)
        {
            Name = "Swapper";
            ImpostorText = () => "Swap The Votes Of Two People";
            TaskText = () => "Swap two people's votes to save <color=#00FFFF>Crewmates</color>!";
            Color = Patches.Colors.Swapper;
            RoleType = RoleEnum.Swapper;
            Faction = Faction.Crewmates;
            Alignment = Alignment.CrewmatePower;
            AddToRoleHistory(RoleType);
        }
    }
}