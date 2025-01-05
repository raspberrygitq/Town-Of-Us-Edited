using System.Linq;
using UnityEngine;
using TownOfUs.Extensions;
using System.Collections.Generic;

namespace TownOfUs.Roles
{
    public class Infectious : Role
    {
        private KillButton _infectButton;
        public bool Enabled;
        public bool InfectiousWins;
        public PlayerControl ClosestPlayer;
        public PlayerControl ClosestPlayerInfect;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public List<byte> Infected = new List<byte>();


        public Infectious(PlayerControl player) : base(player)
        {
            Name = "Infectious";
            ImpostorText = () => "Spread The Virus Everywhere!";
            TaskText = () => "Infect everyone\nFake Tasks:";
            Color = Patches.Colors.Infectious;
            RoleType = RoleEnum.Infectious;
            Cooldown = CustomGameOptions.InfectiousCD;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralKilling;
        }

        public KillButton InfectButton
        {
            get => _infectButton;
            set
            {
                _infectButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        internal override void NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return;

            var alivecrewkiller = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && x.IsCrewKiller() && !x.Data.IsDead && !x.Data.Disconnected).ToList();

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling) || x.Is(Faction.Coven))) == 1 && (alivecrewkiller.Count <= 0 || !CustomGameOptions.CrewKillersContinue))
            {
                Utils.Rpc(CustomRPC.InfectiousWin, Player.PlayerId);
                Wins();
                Utils.EndGame();
                System.Console.WriteLine("GAME OVER REASON: Infectious Win");
                return;
            }

            return;
        }

        public void Wins()
        {
            InfectiousWins = true;
            if (AmongUsClient.Instance.AmHost) Utils.EndGame();
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var infectiousTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            infectiousTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = infectiousTeam;
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
