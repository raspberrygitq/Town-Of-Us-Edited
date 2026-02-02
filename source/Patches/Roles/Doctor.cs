using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Doctor : Role
    {
        public Doctor(PlayerControl owner) : base(owner)
        {
            Name = "Doctor";
            ImpostorText = () => "Revive The <color=#00FFFF>Crewmates</color>";
            if (CustomGameOptions.GameMode == GameMode.Chaos)
            {
                TaskText = () => "Revive the <color=#00FFFF>Crewmates</color> and finish your tasks";
            }
            else
            {
                TaskText = () => "Revive the <color=#00FFFF>Crewmates</color>";
            }
            Color = Patches.Colors.Doctor;
            Cooldown = CustomGameOptions.DocReviveCooldown;
            RoleType = RoleEnum.Doctor;
            AddToRoleHistory(RoleType);
            Faction = Faction.Crewmates;
            Alignment = Alignment.CrewmateProtective;

            UsesLeft = CustomGameOptions.MaxRevives;
        }
        public List<PlayerControl> RevivedList { get; set; } = new List<PlayerControl>();
        public TextMeshPro UsesText;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft != 0;
        public DeadBody CurrentTarget;
        public KillButton _dragDropButton;
        public DeadBody CurrentlyDragging { get; set; }
        public KillButton DragDropButton
        {
            get => _dragDropButton;
            set
            {
                _dragDropButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float ReviveTimer()
        {
            if (!coolingDown) return 0f;
            else if (CustomGameOptions.GameMode == GameMode.Chaos) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }
        public bool CanRevive() //Code from Launchpad Reloaded (All Of Us)
        {
            if (!CustomGameOptions.OnlyMedRevive)
            {
                return true;
            }

            try
            {
                return ShipStatus.Instance.FastRooms[SystemTypes.MedBay].roomArea.OverlapPoint(PlayerControl.LocalPlayer.GetTruePosition());
            }
            catch
            {
                try
                {
                    return ShipStatus.Instance.FastRooms[SystemTypes.Laboratory].roomArea.OverlapPoint(PlayerControl.LocalPlayer.GetTruePosition());
                }
                catch
                {
                    return ShipStatus.Instance.FastRooms[SystemTypes.Medical].roomArea.OverlapPoint(PlayerControl.LocalPlayer.GetTruePosition());
                }
            }

        }
    }
}