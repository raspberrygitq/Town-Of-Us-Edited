using System.Collections.Generic;
using System.Linq;
using TownOfUsEdited.CrewmateRoles.ClericMod;
using TownOfUsEdited.CrewmateRoles.MedicMod;
using TownOfUsEdited.Patches;
using UnityEngine;

namespace TownOfUsEdited.Roles
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
                Cooldown = CustomGameOptions.TempSaveCdReset;
                return;
            }
            else if (interact[3] == true) return;
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
                if (!player.Is(RoleEnum.Pestilence) && !player.IsShielded() && !player.IsProtected() && !player.IsBarriered() && player != ShowShield.FirstRoundShielded)
                {
                    Utils.RpcMultiMurderPlayer(Player, player);
                }
                else if (player.IsShielded())
                {
                    foreach (var medic in player.GetMedic())
                    {
                        Utils.Rpc(CustomRPC.AttemptSound, medic.Player.PlayerId, player.PlayerId);
                        StopKill.BreakShield(medic.Player.PlayerId, player.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                }
                else if (player.IsBarriered())
                {
                    foreach (var cleric in player.GetCleric())
                    {
                        StopAttack.NotifyCleric(cleric.Player.PlayerId, false);
                    }
                }
            }
            //Making sure the Hexed list is cleared to prevent issues
            Hexed.Clear();
            Utils.Rpc(CustomRPC.ClearHex, PlayerControl.LocalPlayer.PlayerId);
            Cooldown = CustomGameOptions.CovenKCD;
        }
    }
}