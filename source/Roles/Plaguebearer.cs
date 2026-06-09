using Reactor.Utilities;
using System.Collections.Generic;
using System.Linq;
using TownOfUsEdited.Extensions;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Plaguebearer : Role
    {
        public PlayerControl ClosestPlayer;
        public List<byte> InfectedPlayers = new List<byte>();
        public bool coolingDown => Cooldown > 0f;
        public float Cooldown;
        public bool PlaguebearerWins { get; set; }

        public int InfectedAlive => InfectedPlayers.Count(x => Utils.PlayerById(x) != null && Utils.PlayerById(x).Data != null && !Utils.PlayerById(x).Data.IsDead && !Utils.PlayerById(x).Data.Disconnected);
        public bool CanTransform => PlayerControl.AllPlayerControls.ToArray().Count(x => x != null && !x.Data.IsDead && !x.Data.Disconnected) <= InfectedAlive;

        public Plaguebearer(PlayerControl player) : base(player)
        {
            Name = "Plaguebearer";
            ImpostorText = () => $"Infect Everyone To Become {Patches.Colors.Pestilence.ToTextColor()}Pestilence</color>";
            TaskText = () => $"Infect everyone to become {Patches.Colors.Pestilence.ToTextColor()}Pestilence</color>\nFake Tasks:";
            Color = Patches.Colors.Plaguebearer;
            RoleType = RoleEnum.Plaguebearer;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralKilling;
            InfectedPlayers.Add(player.PlayerId);
            Cooldown = CustomGameOptions.InfectCd;
        }

        internal override void NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return;

            var alivecrewkiller = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && x.IsCrewKiller() && !x.Data.IsDead && !x.Data.Disconnected).ToList();

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling) || x.Is(Faction.Coven))) == 1 && (alivecrewkiller.Count <= 0 || !CustomGameOptions.CrewKillersContinue))
            {
                Utils.Rpc(CustomRPC.PlaguebearerWin, Player.PlayerId);
                Wins();
                Utils.EndGame();
                PluginSingleton<TownOfUsEdited>.Instance.Log.LogMessage("GAME OVER REASON: Plaguebearer Win");
                return;
            }

            return;
        }

        public void Wins()
        {
            PlaguebearerWins = true;
            if (AmongUsClient.Instance.AmHost) Utils.EndGame();
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var plaguebearerTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            plaguebearerTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = plaguebearerTeam;
        }

        public float InfectTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }

        public void RpcSpreadInfection(PlayerControl source, PlayerControl target)
        {
            new WaitForSeconds(1f);
            SpreadInfection(source, target);
            Utils.Rpc(CustomRPC.Infect, Player.PlayerId, source.PlayerId, target.PlayerId);
        }

        public void SpreadInfection(PlayerControl source, PlayerControl target)
        {
            if (InfectedPlayers.Contains(source.PlayerId) && !InfectedPlayers.Contains(target.PlayerId)) InfectedPlayers.Add(target.PlayerId);
            else if (InfectedPlayers.Contains(target.PlayerId) && !InfectedPlayers.Contains(source.PlayerId)) InfectedPlayers.Add(source.PlayerId);
        }

        public void TurnPestilence()
        {
            var oldRole = GetRole(Player);
            var killsList = (oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
            RoleDictionary.Remove(Player.PlayerId);
            var role = new Pestilence(Player);
            role.CorrectAssassinKills = killsList.CorrectAssassinKills;
            role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
            if (Player == PlayerControl.LocalPlayer)
            {
                Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Pestilence));
                role.RegenTask();
            }
        }
    }
}