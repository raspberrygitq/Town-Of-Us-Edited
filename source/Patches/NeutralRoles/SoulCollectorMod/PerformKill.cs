using HarmonyLib;
using TownOfUsEdited.Roles;
using System.Collections;
using UnityEngine;
using Reactor.Utilities.Extensions;

namespace TownOfUsEdited.NeutralRoles.SoulCollectorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != HudManager.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.SoulCollector);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return false;
            var role = Role.GetRole<SoulCollector>(PlayerControl.LocalPlayer);
            if (role.Player.inVent) return false;
            if (role.Cooldown > 0) return false;

            if (role.ClosestPlayer == null) return false;
            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
            if (interact[4] == true) return false;
            else if (interact[0] == true)
            {
                role.Cooldown = CustomGameOptions.ReapCd;
                return false;
            }
            else if (interact[1] == true)
            {
                role.Cooldown = CustomGameOptions.TempSaveCdReset;
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }

        public static IEnumerator RemoveBody(DeadBody body)
        {
            SpriteRenderer renderer = null;
            foreach (var body2 in body.bodyRenderers) renderer = body2;
            var colour = renderer.color;
            renderer.color = new Color(colour.r, colour.g, colour.b, 0f);
            yield return new WaitForSeconds(1f);
            body.gameObject.Destroy();
        }
    }
}