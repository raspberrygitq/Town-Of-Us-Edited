using System.Linq;
using System.Collections.Generic;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Patches;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class HexMaster : Role
    {
        public HexMaster(PlayerControl owner) : base(owner)
        {
            Name = "Hex Master";
            ImpostorText = () => "Hex Everyone";
            TaskText = () => "Hex all non Coven members\nFake Tasks:";
            Color = Patches.Colors.Coven;
            RoleType = RoleEnum.HexMaster;
            AddToRoleHistory(RoleType);
            Faction = Faction.Coven;
            Cooldown = CustomGameOptions.CovenKCD;
        }

        public List<byte> Hexed = new List<byte>();
        public KillButton _sabotageButton;
        public KillButton _hexBombButton;
        public PlayerControl ClosestPlayer;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public void Hex(PlayerControl target)
        {
            // Check if the Hex Master can hex
            if (Cooldown > 0)
                return;

            if (target.Is(Faction.Coven))
                return;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, target);
            if (interact[4] == true)
            {
                Hexed.Add(target.PlayerId);
                Utils.Rpc(CustomRPC.Hex, PlayerControl.LocalPlayer.PlayerId, target.PlayerId);
                Cooldown = CustomGameOptions.CovenKCD;
                return;
            }
            if (interact[0] == true)
            {
                Cooldown = CustomGameOptions.ProtectKCReset;
                return;
            }
            else if (interact[1] == true)
            {
                Cooldown = CustomGameOptions.VestKCReset;
                return;
            }
            else if (interact[3] == true) return;
        }
        public KillButton SabotageButton
        {
            get => _sabotageButton;
            set
            {
                _sabotageButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public KillButton HexBombButton
        {
            get => _hexBombButton;
            set
            {
                _hexBombButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
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
            var covenTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            covenTeam.Add(PlayerControl.LocalPlayer);
            var toAdd = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Coven) && x != PlayerControl.LocalPlayer).ToList();
            foreach (var player in toAdd)
            {
                covenTeam.Add(player);
            }
            __instance.teamToShow = covenTeam;
        }

        public void HexBomb()
        {
            foreach (var playerId in Hexed)
            {
                var player = Utils.PlayerById(playerId);
                if (!player.Is(RoleEnum.Pestilence) && !player.IsShielded() && !player.IsProtected() && player != ShowRoundOneShield.FirstRoundShielded)
                {
                    Utils.RpcMultiMurderPlayer(Player, player);
                }
                else if (player.IsShielded())
                {
                    var medic = player.GetMedic().Player.PlayerId;
                    Utils.Rpc(CustomRPC.AttemptSound, medic, player.PlayerId);
                    StopKill.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
            }
            //Making sure the Hexed list is cleared to prevent issues
            Hexed.Clear();
            Utils.Rpc(CustomRPC.ClearHex, PlayerControl.LocalPlayer.PlayerId);
            Cooldown = CustomGameOptions.CovenKCD;
        }
    }
}