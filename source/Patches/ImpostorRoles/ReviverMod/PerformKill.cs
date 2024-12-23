using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.Patches.ImpostorRoles.ReviverMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Reviver))
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            var role = Role.GetRole<Reviver>(PlayerControl.LocalPlayer);
            
            if (role.CurrentTarget == null)
                return false;

            var player = Utils.PlayerById(role.CurrentTarget.ParentId);
            if (player.Is(RoleEnum.Reviver)) return false;
                
            if (__instance == role.ReviveButton) 
            {
                if (role.CurrentTarget.IsDouble())
                    return false;
                    
                if (!__instance.isActiveAndEnabled) return false;
                var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];

                if (Vector2.Distance(role.CurrentTarget.TruePosition,
                    PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;

                role.ReviveAbility(role.CurrentTarget);
                return false;
            }

            return false;
        }
    }
}