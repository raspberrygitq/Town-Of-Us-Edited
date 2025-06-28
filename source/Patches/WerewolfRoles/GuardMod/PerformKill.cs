using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.WerewolfRoles.GuardMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static Color ProtectedColor = Color.cyan;
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Guard))
                return true;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            var role = Role.GetRole<Guard>(PlayerControl.LocalPlayer);

            var ProtectButton = DestroyableSingleton<HudManager>.Instance.KillButton;
                
            if (__instance == ProtectButton)
            {
                if (role.ClosestPlayer == null)
                return false;

                if (!__instance.isActiveAndEnabled)
                return false;

                role.ProtectedPlayer = role.ClosestPlayer;
                Utils.Rpc(CustomRPC.SetGuard, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
                role.UsedProtect = true;
                return false;
            }

            return false;
        }
    }
}