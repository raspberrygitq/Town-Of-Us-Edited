using Reactor.Utilities;
using System;
using TownOfUsEdited.CrewmateRoles.TimeLordMod;
using TownOfUsEdited.Patches;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Escapist : Role
    {
        public KillButton _escapeButton;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public int UsesLeft;
        public Vector3 EscapePoint = new();

        public Escapist(PlayerControl player) : base(player)
        {
            Name = "Escapist";
            ImpostorText = () => "Get Away From Kills With Ease";
            TaskText = () => "Teleport to get away from bodies\nFake Tasks:";
            Color = Patches.Colors.Impostor;
            RoleType = RoleEnum.Escapist;
            AddToRoleHistory(RoleType);
            Cooldown = CustomGameOptions.EscapeCd;
            Faction = Faction.Impostors;
            Alignment = Alignment.ImpostorConcealing;
        }

        public KillButton EscapeButton
        {
            get => _escapeButton;
            set
            {
                _escapeButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float EscapeTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }
        public static void Escape(PlayerControl escapist)
        {
            escapist.MyPhysics.ResetMoveState();
            var escapistRole = Role.GetRole<Escapist>(escapist);
            if (escapistRole.EscapePoint == Vector3.zero) return;
            var position2 = PlayerControl.LocalPlayer.transform.position;
            TimeLordPatches.Positions.Add((Vector2.zero, Time.time, "Teleport", position2, 0, null));
            var position = escapistRole.EscapePoint;
            escapist.NetTransform.SnapTo(new Vector2(position.x, position.y));

            if (SubmergedCompatibility.isSubmerged())
            {
                if (PlayerControl.LocalPlayer.PlayerId == escapist.PlayerId)
                {
                    SubmergedCompatibility.ChangeFloor(escapist.GetTruePosition().y > -7);
                    SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                }
            }

            if (PlayerControl.LocalPlayer.PlayerId == escapist.PlayerId)
            {
                Coroutines.Start(Utils.FlashCoroutine(new Color(0.6f, 0.1f, 0.2f, 1f)));
                if (Minigame.Instance) Minigame.Instance.Close();
            }

            escapist.moveable = true;
            escapist.Collider.enabled = true;
            escapist.NetTransform.enabled = true;
        }
    }
}