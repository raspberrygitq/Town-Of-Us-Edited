using System;
using System.Collections.Generic;
using TMPro;
using TownOfUs.NeutralRoles.SoulCollectorMod;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class SoulCollector : Role
    {
        private KillButton _reapButton;
        public PlayerControl ClosestPlayer;
        public Soul CurrentTarget;
        public List<GameObject> Souls = new List<GameObject>();
        public bool CollectedSouls = false;
        public int SoulsCollected = 0;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;

        public List<byte> ReapedPlayers = new List<byte>();
        public TextMeshPro CollectedText { get; set; }

        public SoulCollector(PlayerControl player) : base(player)
        {
            Name = "Soul Collector";
            ImpostorText = () => "Collect Souls";
            TaskText = () => "Collect souls to win the game";
            Color = Patches.Colors.SoulCollector;
            RoleType = RoleEnum.SoulCollector;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralEvil;
            Cooldown = CustomGameOptions.ReapCd;
        }

        public KillButton ReapButton
        {
            get => _reapButton;
            set
            {
                _reapButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float ReapTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }

        public void Wins()
        {
            CollectedSouls = true;
            if (AmongUsClient.Instance.AmHost && CustomGameOptions.NeutralEvilWinEndsGame) Utils.EndGame();
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var scTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            scTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = scTeam;
        }
    }
}