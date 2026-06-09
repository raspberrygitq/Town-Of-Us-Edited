using Reactor.Utilities;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Hunter : Role
    {
        public Hunter(PlayerControl player) : base(player)
        {
            Name = "Hunter";
            ImpostorText = () => "Stalk The <color=#FF0000FF>Impostor</color>";
            TaskText = () => "Stalk and kill <color=#FF0000>Impostors</color>, but not <color=#00FFFF>Crewmates</color>";
            Color = Patches.Colors.Hunter;
            Cooldown = CustomGameOptions.HunterKillCd;
            RoleType = RoleEnum.Hunter;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            Alignment = Alignment.CrewmateKilling;
            UsesLeft = CustomGameOptions.HunterStalkUses;
        }

        private KillButton _stalkButton;
        public PlayerControl ClosestPlayer;
        public PlayerControl ClosestStalkPlayer;
        public PlayerControl StalkedPlayer;
        public List<PlayerControl> CaughtPlayers = new List<PlayerControl>();
        public bool Enabled { get; set; }
        public float StalkCooldown;
        public bool StalkcoolingDown => StalkCooldown > 0f;
        public float StalkDuration { get; set; }
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public int UsesLeft { get; set; }
        public TextMeshPro UsesText { get; set; }
        public bool UsedRetribution { get; set; } = false;
        public KillButton StalkButton
        {
            get => _stalkButton;
            set
            {
                _stalkButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public bool Stalking => StalkDuration > 0f;
        public bool StalkUsable => UsesLeft != 0;

        public float HunterKillTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }

        public float StalkTimer()
        {
            if (!StalkcoolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                StalkCooldown -= Time.deltaTime;
                return StalkCooldown;
            }
            else return StalkCooldown;
        }

        public void Stalk()
        {
            Enabled = true;
            StalkDuration -= Time.deltaTime;
        }

        public void StopStalking()
        {
            Enabled = false;
            StalkCooldown = CustomGameOptions.HunterStalkCd;
            StalkedPlayer = null;
        }

        public void RpcCatchPlayer(PlayerControl stalked)
        {
            if (PlayerControl.LocalPlayer.PlayerId == Player.PlayerId)
            {
                Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Hunter));
            }
            CaughtPlayers.Add(stalked);
            StalkDuration = 0;
            StopStalking();
        }
    }
}