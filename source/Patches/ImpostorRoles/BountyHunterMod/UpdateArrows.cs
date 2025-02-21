using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.ImpostorRoles.BountyHunterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class UpdateArrows
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.BountyHunter)) return;

            var role = Role.GetRole<BountyHunter>(PlayerControl.LocalPlayer);

            if (PlayerControl.LocalPlayer.Data.IsDead)
            {
                role.TargetArrow.Values.DestroyAll();
                role.TargetArrow.Clear();
                role.BountyTarget = null;
                return;
            }

            foreach (var arrow in role.TargetArrow)
            {
                var player = Utils.PlayerById(arrow.Key);
                if (player == null || player.Data == null || player.Data.IsDead || player.Data.Disconnected)
                {
                    role.DestroyArrow(arrow.Key);
                    continue;
                }
                arrow.Value.image.color = Palette.ImpostorRed;
                arrow.Value.target = player.transform.position;
            }
        }
    }
}