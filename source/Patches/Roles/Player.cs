using System;
using System.Linq;
using Il2CppSystem.Collections.Generic;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Player : Role
    {
        public Player(PlayerControl owner) : base(owner)
        {
            Name = "Player";
            ImpostorText = () => "Kill Everyone And Win Alone";
            TaskText = () => "Kill everyone and survive";
            Color = Patches.Colors.Player;
            Cooldown = CustomGameOptions.BattleRoyaleKillCD;
            RoleType = RoleEnum.Player;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralKilling;
        }

        public PlayerControl ClosestPlayer;
        public bool coolingDown => Cooldown > 0f;
        public float Cooldown;
        public bool PlayerWins { get; set; }

        internal override void NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling))) == 1)
            {
                Utils.Rpc(CustomRPC.PlayerWin, Player.PlayerId);
                Wins();
                Utils.EndGame();
                System.Console.WriteLine("GAME OVER REASON: Player Win (Battle Royale)");
                return;
            }

            return;
        }
        public void Wins()
        {
            //System.Console.WriteLine("Reached Here - Player Edition");
            PlayerWins = true;
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

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var PlayerTeam = new List<PlayerControl>();
            PlayerTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = PlayerTeam;
        }
    }
}