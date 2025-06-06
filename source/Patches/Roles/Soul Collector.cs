using System.Collections.Generic;
using System.Linq;
using TMPro;
using TownOfUsEdited.Extensions;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class SoulCollector : Role
    {
        public PlayerControl ClosestPlayer;
        public List<GameObject> Souls = new List<GameObject>();
        public bool SCWins = false;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;

        public SoulCollector(PlayerControl player) : base(player)
        {
            Name = "Soul Collector";
            ImpostorText = () => "Reap Souls";
            TaskText = () => "Reap all souls\nFake Tasks:";
            Color = Patches.Colors.SoulCollector;
            RoleType = RoleEnum.SoulCollector;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralKilling;
            Cooldown = CustomGameOptions.ReapCd;
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
            SCWins = true;
        }

        internal override void NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return;
            var alivecrewkiller = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && x.IsCrewKiller() && !x.Data.IsDead && !x.Data.Disconnected).ToList();

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling) || x.Is(Faction.Coven))) == 1 && (alivecrewkiller.Count <= 0 || !CustomGameOptions.CrewKillersContinue))
            {
                Utils.Rpc(CustomRPC.SoulCollectorWin, Player.PlayerId);
                Wins();
                Utils.EndGame();
                return;
            }
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var scTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            scTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = scTeam;
        }
    }
}