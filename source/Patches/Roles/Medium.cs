using Reactor.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Medium : Role
    {
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;

        public Dictionary<byte, ArrowBehaviour> MediatedPlayers = new Dictionary<byte, ArrowBehaviour>();
        
        public static Sprite Arrow => TownOfUsEdited.Arrow;
        
        public Medium(PlayerControl player) : base(player)
        {
            Name = "Medium";
            ImpostorText = () => "Watch The Spooky Ghosts";
            TaskText = () => "Follow ghosts to get clues from them";
            Color = Patches.Colors.Medium;
            Cooldown = CustomGameOptions.MediateCooldown;
            RoleType = RoleEnum.Medium;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            Alignment = Alignment.CrewmateSupport;
            Scale = 1.4f;
            MediatedPlayers = new Dictionary<byte, ArrowBehaviour>();
        }

        internal override bool RoleCriteria()
        {
            return (MediatedPlayers.ContainsKey(PlayerControl.LocalPlayer.PlayerId) && CustomGameOptions.ShowMediumToDead) || base.RoleCriteria();
        }

        public float MediateTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }

        public void AddMediatePlayer(byte playerId)
        {
            var gameObj = new GameObject();
            var arrow = gameObj.AddComponent<ArrowBehaviour>();
            if (Player.PlayerId == PlayerControl.LocalPlayer.PlayerId || CustomGameOptions.ShowMediumToDead)
            {
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = Arrow;
                arrow.image = renderer;
                gameObj.layer = 5;
                arrow.target = Utils.PlayerById(playerId).transform.position;
            }
            MediatedPlayers.Add(playerId, arrow);
            Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Medium));
        }
    }
}