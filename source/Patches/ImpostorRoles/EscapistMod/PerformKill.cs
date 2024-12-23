using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.EscapistMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static Sprite MarkSprite => TownOfUs.MarkSprite;
        public static Sprite EscapeSprite => TownOfUs.EscapeSprite;

        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Escapist);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Escapist>(PlayerControl.LocalPlayer);
            if (__instance == role.EscapeButton)
            {
                if (role.Player.inVent) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.EscapeButton.graphic.sprite == MarkSprite)
                {
                    var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                    if (!abilityUsed) return false;
                    role.EscapePoint = PlayerControl.LocalPlayer.transform.position;
                    role.EscapeButton.graphic.sprite = EscapeSprite;
                    DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
                    if (role.Cooldown < 5f)
                        role.Cooldown = 5f;
                }
                else
                {
                    if (__instance.isCoolingDown) return false;
                    if (role.Cooldown > 0) return false;
                    var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                    if (!abilityUsed) return false;
                    Utils.Rpc(CustomRPC.Escape, PlayerControl.LocalPlayer.PlayerId, role.EscapePoint);
                    role.Cooldown = CustomGameOptions.EscapeCd;
                    Escapist.Escape(role.Player);
                }

                return false;
            }

            return true;
        }
    }
}
