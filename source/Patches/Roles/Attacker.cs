using System.Linq;
using Reactor.Utilities;
using TownOfUsEdited.Extensions;

namespace TownOfUsEdited.Roles
{
    public class Attacker : Role
    {
        public bool AttackerWins { get; set; }

        public Attacker(PlayerControl player) : base(player)
        {
            Name = "Attacker";
            ImpostorText = () => "Spread Fear Among The Players";
            TaskText = () => "Complete Your Tasks To Become Terrorist\nFake Tasks:";
            Color = Patches.Colors.Attacker;
            RoleType = RoleEnum.Attacker;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralKilling;
        }

        internal override void NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return;

            var alivecrewkiller = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && x.IsCrewKiller() && !x.Data.IsDead && !x.Data.Disconnected).ToList();

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling) || x.Is(Faction.Coven))) == 1 && (alivecrewkiller.Count <= 0 || !CustomGameOptions.CrewKillersContinue))
            {
                Utils.Rpc(CustomRPC.AttackerWin, Player.PlayerId);
                Wins();
                System.Console.WriteLine("GAME OVER REASON: Attacker Win");
                return;
            }

            return;
        }

        public void Wins()
        {
            AttackerWins = true;
            if (AmongUsClient.Instance.AmHost) Utils.EndGame();
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var attackerTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            attackerTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = attackerTeam;
        }

        public void TurnTerrorist()
        {
            var oldRole = GetRole(Player);
            var killsList = (oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
            RoleDictionary.Remove(Player.PlayerId);
            var role = new Terrorist(Player);
            role.CorrectAssassinKills = killsList.CorrectAssassinKills;
            role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
            if (Player == PlayerControl.LocalPlayer)
            {
                Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Terrorist));
                role.RegenTask();
            }
        }
    }
}