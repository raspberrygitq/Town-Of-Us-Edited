using TownOfUsEdited.Patches;
using UnityEngine;

namespace TownOfUsEdited.Roles.Modifiers
{
    public class Motionless : Modifier
    {
        public Motionless(PlayerControl player) : base(player)
        {
            Name = "Motionless";
            TaskText = () => "You will not move when a meeting is called";
            Color = Patches.Colors.Motionless;
            ModifierType = ModifierEnum.Motionless;
        }

        public Vector3 Position { get; set; }
        public bool inVent = false;
        public int VentId;

        public void ResetPosition()
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (Position != Vector3.zero)
            {
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Position);
                if (SubmergedCompatibility.isSubmerged())
                {
                    SubmergedCompatibility.ChangeFloor(PlayerControl.LocalPlayer.GetTruePosition().y > -7);
                    SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                }

                Position = Vector3.zero;

                if (inVent)
                {
                    PlayerControl.LocalPlayer.MyPhysics.RpcEnterVent(VentId);
                    inVent = false;
                }
            }
        }
    }
}