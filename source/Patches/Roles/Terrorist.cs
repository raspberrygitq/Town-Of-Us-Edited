using System;
using System.Linq;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles
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

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling) || x.Is(Faction.Coven) || x.IsCrewKiller())) == 1)
            {
                Utils.Rpc(CustomRPC.TerroristWin, Player.PlayerId);
                Wins();
                Utils.EndGame();
                System.Console.WriteLine("GAME OVER REASON: Terrorist Win");
                return;
            }

            return;
        }

        public void Wins()
        {
            TerroristWins = true;
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