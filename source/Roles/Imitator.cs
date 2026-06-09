using System.Collections.Generic;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Imitator : Role
    {
        public readonly List<GameObject> Buttons = new List<GameObject>();

        public readonly List<(byte, bool)> ListOfActives = new List<(byte, bool)>();
        public PlayerControl ImitatePlayer = null;

        public List<RoleEnum> trappedPlayers = null;
        public PlayerControl confessingPlayer = null;


        public Imitator(PlayerControl player) : base(player)
        {
            Name = "Imitator";
            ImpostorText = () => "Use The True-Hearted Dead To Benefit <color=#00FFFF>Crewmates</color>";
            TaskText = () => "Use dead roles to benefit <color=#00FFFF>Crewmates</color>";
            Color = Patches.Colors.Imitator;
            RoleType = RoleEnum.Imitator;
            Alignment = Alignment.CrewmateSupport;
            AddToRoleHistory(RoleType);
        }
    }
}