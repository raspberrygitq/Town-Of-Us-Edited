using Reactor.Utilities;
using System.Linq;
using TownOfUsEdited.Extensions;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Terrorist : Role
    {
        public Terrorist(PlayerControl owner) : base(owner)
        {
            Name = "Terrorist";
            Color = Patches.Colors.Terrorist;
            Cooldown = CustomGameOptions.TerroristKillCD;
            RoleType = RoleEnum.Terrorist;
            AddToRoleHistory(RoleType);
            ImpostorText = () => "Uhm... How Are You Seeing This?";
            TaskText = () => "Kill everyone with your unstoppable abilities!\nFake Tasks:";
            Faction = Faction.NeutralKilling;
        }

        public PlayerControl ClosestPlayer;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public bool TerroristWins { get; set; }
        public bool SavedVote = false;

        internal override void NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return;

            var alivecrewkiller = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && x.IsCrewKiller() && !x.Data.IsDead && !x.Data.Disconnected).ToList();

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling) || x.Is(Faction.Coven))) == 1 && (alivecrewkiller.Count <= 0 || !CustomGameOptions.CrewKillersContinue))
            {
                Utils.Rpc(CustomRPC.TerroristWin, Player.PlayerId);
                Wins();
                Utils.EndGame();
                PluginSingleton<TownOfUsEdited>.Instance.Log.LogMessage("GAME OVER REASON: Terrorist Win");
                return;
            }

            return;
        }

        public void Wins()
        {
            TerroristWins = true;
            if (AmongUsClient.Instance.AmHost) Utils.EndGame();
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