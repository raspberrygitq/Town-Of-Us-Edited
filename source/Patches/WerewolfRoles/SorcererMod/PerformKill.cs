using HarmonyLib;
using TownOfUs.Roles;
using Reactor.Utilities;
using AmongUs.GameOptions;
using UnityEngine;

namespace TownOfUs.WerewolfRoles.SorcererMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Sorcerer))
                return true;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            var role = Role.GetRole<Sorcerer>(PlayerControl.LocalPlayer);

            var PoisonButton = DestroyableSingleton<HudManager>.Instance.KillButton;
                
            if (__instance == PoisonButton)
            {
                if (role.ClosestPlayer == null)
                return false;

                if (!__instance.isActiveAndEnabled)
                return false;

                role.PoisonedPlayer = role.ClosestPlayer;
                role.UsedPoison = true;
                return false;
            }

            if (__instance == role.ReviveButton)
            {
                if (!__instance.enabled) return false;
                var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
                if (role == null)
                return false;
                if (role.CurrentTarget == null)
                return false;
                if (Vector2.Distance(role.CurrentTarget.TruePosition,
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                var playerId = role.CurrentTarget.ParentId;
                var player = Utils.PlayerById(playerId);

                Utils.Rpc(CustomRPC.SorcererRevive, PlayerControl.LocalPlayer.PlayerId, playerId);

                Coroutines.Start(Coroutine.SorcererRevive(role.CurrentTarget, role));
                role.UsedRevive = true;
                return false;
            }

            return false;
        }
    }
}