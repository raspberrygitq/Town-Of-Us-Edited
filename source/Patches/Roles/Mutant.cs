using System;
using System.Linq;
using Reactor.Utilities;
using TMPro;
using TownOfUsEdited.Extensions;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Mutant : Role
    {
        public Mutant(PlayerControl owner) : base(owner)
        {
            Name = "Mutant";
            ImpostorText = () => "Transform and kill the players";
            TaskText = () => "<color=#B6EB5E>Transform to get a shorter kill cooldown.\nFake Tasks:</color>";
            Color = Patches.Colors.Mutant;
            RoleType = RoleEnum.Mutant;
            TransformCooldown = CustomGameOptions.TransformCD;
            Cooldown = CustomGameOptions.MutantKCD;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralKilling;
        }

        public PlayerControl ClosestPlayer;
        public KillButton _transformButton;
        public TextMeshPro TransformText;
        public bool coolingDown => Cooldown > 0f;
        public float Cooldown;
        public bool TransformcoolingDown => TransformCooldown > 0f;
        public float TransformCooldown;
        public bool MutantWins { get; set; }
        public bool IsTransformed { get; set; } = false;
        public KillButton TransformButton
        {
            get => _transformButton;
            set
            {
                _transformButton = value;
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
                Utils.Rpc(CustomRPC.MutantWin, Player.PlayerId);
                Wins();
                Utils.EndGame();
                PluginSingleton<TownOfUsEdited>.Instance.Log.LogMessage("GAME OVER REASON: Mutant Win");
                return;
            }

            return;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var mutantTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            mutantTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = mutantTeam;
        }

        public void Wins()
        {
            MutantWins = true;
            if (AmongUsClient.Instance.AmHost) Utils.EndGame();
        }
        public void Kill(PlayerControl target)
        {
            // Check if the Serial Killer can kill
            if (Cooldown > 0)
                return;

            Utils.Interact(PlayerControl.LocalPlayer, target, true);
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
        public float TransformTimer()
        {
            if (!TransformcoolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                TransformCooldown -= Time.deltaTime;
                return TransformCooldown;
            }
            else return TransformCooldown;
        }
    }
}