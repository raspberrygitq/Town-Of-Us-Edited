using System;
using System.Linq;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Fighter : Role
    {
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;

        public PlayerControl ClosestPlayer;
        public bool SpawnedAs = true;

        public Fighter(PlayerControl player) : base(player)
        {
            Name = "Fighter";
            ImpostorText = () => "Hero? or Traitor?";
            TaskText = () => SpawnedAs ? "You can kill anyone but be careful.\nDon't kill a <color=#00FFFF>Crewmate</color> or else..." : "Imagine betraying the <color=#00FFFF>Crewmates</color> and becoming impo again...";
            Color = Patches.Colors.Fighter;
            Cooldown = CustomGameOptions.FighterKCD;
            Faction = Faction.Crewmates;
            RoleType = RoleEnum.Fighter;
            Alignment = Alignment.CrewmateKilling;
            AddToRoleHistory(RoleType);
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

        public void Kill(PlayerControl target)
        {
            if (Cooldown > 0)
                return;

            Utils.Interact(PlayerControl.LocalPlayer, target, true);

            if (PlayerControl.LocalPlayer.Data.IsDead) return;

            Cooldown = CustomGameOptions.FighterKCD;

            if (PlayerControl.LocalPlayer.Is(Faction.Madmates)) return;

            if (ClosestPlayer.Is(Faction.Crewmates) || (ClosestPlayer.Is(Faction.NeutralBenign) && !CustomGameOptions.FighterKillsNB)
            || (ClosestPlayer.Is(Faction.NeutralEvil) && !CustomGameOptions.FighterKillsNE)
            || (ClosestPlayer.Is(Faction.NeutralKilling) && !CustomGameOptions.FighterKillsNK)
            || (ClosestPlayer.Is(Faction.Coven) && !CustomGameOptions.FighterKillsCoven)
            || (ClosestPlayer.Is(Faction.Madmates) && !CustomGameOptions.FighterKillsMadmate))
            {
                Utils.Rpc(CustomRPC.ImpConvert, PlayerControl.LocalPlayer.PlayerId);
                Utils.TurnMadmate(PlayerControl.LocalPlayer, false);
            }
        }
    }
}