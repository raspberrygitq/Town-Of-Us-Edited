using System.Linq;
using Il2CppSystem.Collections.Generic;
using Reactor.Utilities;
using TownOfUsEdited.Extensions;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Vampire : Role
    {
        public Vampire(PlayerControl player) : base(player)
        {
            Name = "Vampire";
            ImpostorText = () => "Convert Crewmates And Kill The Rest";
            TaskText = () => "Bite all other players\nFake Tasks:";
            Color = Patches.Colors.Vampire;
            Cooldown = CustomGameOptions.BiteCd;
            RoleType = RoleEnum.Vampire;
            Faction = Faction.NeutralKilling;
            AddToRoleHistory(RoleType);
        }

        public bool SpawnedAs = true;
        public Role OldRole;
        public PlayerControl ClosestPlayer;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public KillButton _biteButton;

        public float BiteTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }

        public KillButton BiteButton
        {
            get => _biteButton;
            set
            {
                _biteButton = value;
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
                VampWin();
                PluginSingleton<TownOfUsEdited>.Instance.Log.LogMessage("GAME OVER REASON: Vampire Win");
                return;
            }
            else if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 4 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling) || x.Is(Faction.Coven)) && !x.Is(RoleEnum.Vampire)) == 0 && (alivecrewkiller.Count <= 0 || !CustomGameOptions.CrewKillersContinue))
            {
                var vampsAlives = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(RoleEnum.Vampire)).ToList();
                if (vampsAlives.Count == 1) return;
                VampWin();
                PluginSingleton<TownOfUsEdited>.Instance.Log.LogMessage("GAME OVER REASON: Vampire Win");
                return;
            }
            else
            {
                var alivelovers = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(ModifierEnum.Lover)).ToList();
                var vampsAlives = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(RoleEnum.Vampire)).ToList();
                if (vampsAlives.Count == 1 || vampsAlives.Count == 2) return;
                var alives = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();
                var killersAlive = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(RoleEnum.Vampire) && (x.Is(Faction.Impostors) || x.Is(Faction.NeutralKilling) || x.Is(Faction.Coven))).ToList();
                if (killersAlive.Count > 0) return;
                if (alivecrewkiller.Count > 0 && CustomGameOptions.CrewKillersContinue) return;
                if (alives.Count <= 6)
                {
                    VampWin();
                    PluginSingleton<TownOfUsEdited>.Instance.Log.LogMessage("GAME OVER REASON: Vampire Win");
                    return;
                }
                return;
            }
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var vampTeam = new List<PlayerControl>();
            vampTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = vampTeam;
        }
    }
}