using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;
using TMPro;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Tracker : Role
    {
        public Dictionary<byte, ArrowBehaviour> TrackerArrows = new Dictionary<byte, ArrowBehaviour>();
        public PlayerControl ClosestPlayer;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;

        public int UsesLeft;
        public TextMeshPro UsesText;

        public bool ButtonUsable => UsesLeft != 0;

        public Tracker(PlayerControl player) : base(player)
        {
            Name = "Tracker";
            ImpostorText = () => "Track Everyone's Movement";
            TaskText = () => "Track suspicious players";
            Color = Patches.Colors.Tracker;
            Cooldown = CustomGameOptions.TrackCd;
            RoleType = RoleEnum.Tracker;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            Alignment = Alignment.CrewmateInvestigative;

            UsesLeft = CustomGameOptions.MaxTracks;
        }

        public float TrackerTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }

        public bool IsTracking(PlayerControl player)
        {
            return TrackerArrows.ContainsKey(player.PlayerId);
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = TrackerArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
            TrackerArrows.Remove(arrow.Key);
        }
    }
}