using System;
using System.Linq;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class WhiteWolf : Role
    {
        private KillButton _rampageButton;
        public bool Rampaged = false;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public float RampageCooldown;
        public bool RampagecoolingDown => RampageCooldown > 0f;
        public bool WhiteWolfWins = false;
        public PlayerControl ClosestPlayer;
        public WhiteWolf(PlayerControl player) : base(player)
        {
            Name = "White Wolf";
            ImpostorText = () => "Kill Everyone";
            TaskText = () => "Kill all <color=#adf34b>Villagers</color> and <color=#A86629FF>Werewolves</color> to win";
            Color = Patches.Colors.WhiteWolf;
            RoleType = RoleEnum.WhiteWolf;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralKilling;
            Cooldown = CustomGameOptions.WerewolfKillCD;
            RampageCooldown = CustomGameOptions.RampageCD;
        }

        public KillButton RampageButton
        {
            get => _rampageButton;
            set
            {
                _rampageButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void Kill(PlayerControl target)
        {
            // Check if the White Wolf can kill
            if (Cooldown > 0)
                return;

            Utils.Interact(PlayerControl.LocalPlayer, target, true);

            Cooldown = CustomGameOptions.WerewolfKillCD;
        }

        internal override void NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling) || x.Is(Faction.Coven) || x.IsCrewKiller())) == 1)
            {
                Utils.Rpc(CustomRPC.WhiteWolfWin, Player.PlayerId);
                Wins();
                return;
            }

            return;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var whitewolfTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            whitewolfTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = whitewolfTeam;
        }

        public void Wins()
        {
            WhiteWolfWins = true;
        }

        public float RampageTimer()
        {
            if (!RampagecoolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                RampageCooldown -= Time.deltaTime;
                return RampageCooldown;
            }
            else return RampageCooldown;
        }

        public float KillTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }
    }
}