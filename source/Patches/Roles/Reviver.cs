using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using Reactor.Utilities.Extensions;
using TownOfUs.CrewmateRoles.MedicMod;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Reviver : Role
    {
        public PlayerControl RevivedPlayer;
        public KillButton _reviveButton;
        public bool UsedRevive = false;
        public bool CanRevive = true;
        public DeadBody CurrentTarget;

        public Reviver(PlayerControl player) : base(player)
        {
            Name = "Reviver";
            ImpostorText = () => "live... or die.";
            TaskText = () => "Revive inside of a dead player\nFake Tasks:";
            Color = Patches.Colors.Impostor;
            RoleType = RoleEnum.Reviver;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            Alignment = Alignment.ImpostorSupport;
        }

        public KillButton ReviveButton
        {
            get => _reviveButton;
            set
            {
                _reviveButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void ReviveAbility(DeadBody target)
        {
            Revive(target, PlayerControl.LocalPlayer);
            Utils.Rpc(CustomRPC.ReviverRevive, PlayerControl.LocalPlayer.PlayerId, target.ParentId);
        }

        public static void Revive(DeadBody target, PlayerControl player)
        {
            var playerId = target.ParentId;
            var player2 = Utils.PlayerById(playerId);
            var position = target.TruePosition;
            var role = Role.GetRole<Reviver>(player);
            role.UsedRevive = true;
            role.CanRevive = false;

            var revived = new List<PlayerControl>();

            if (target != null)
            {
                foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
                {
                    if (deadBody.ParentId == target.ParentId) deadBody.gameObject.Destroy();
                }
            }

            player.Revive();
            Murder.KilledPlayers.Remove(
                Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == player.PlayerId));
            revived.Add(player);
            player.NetTransform.SnapTo(new Vector2(position.x, position.y + 0.3636f));
            RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);

            if (PlayerControl.LocalPlayer == player) player.myTasks.RemoveAt(1);

            if (Patches.SubmergedCompatibility.isSubmerged() && PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
            {
                Patches.SubmergedCompatibility.ChangeFloor(target.transform.position.y > -7);
            }
            if (target != null) Object.Destroy(target.gameObject);

            if (revived.Any(x => x.AmOwner))
                try
                {
                    Minigame.Instance.Close();
                    Minigame.Instance.Close();
                }
                catch
                {
                }
            Utils.Unmorph(player);
            Utils.Morph(player, player2);
            role.RevivedPlayer = player2;
            return;
        }
    }
}