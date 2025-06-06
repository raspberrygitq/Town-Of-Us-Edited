using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.ImpostorRoles.PoisonerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformPoisonKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Poisoner);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Poisoner>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;
            if (target == null) return false;
            if (!__instance.isActiveAndEnabled) return false;
            if (__instance != HudManager.Instance.KillButton) return false;
            if (role.Cooldown > 0) return false;
            if (role.Enabled == true) return false;
            if (PlayerControl.LocalPlayer.IsControlled() && role.ClosestPlayer.Is(Faction.Coven))
            {
                Utils.Interact(role.ClosestPlayer, PlayerControl.LocalPlayer, true);
                return false;
            }
            var interact = Utils.Interact(PlayerControl.LocalPlayer, target);
            if (interact[4] == true)
            {
                role.PoisonedPlayer = target;
                role.TimeRemaining = CustomGameOptions.PoisonDuration;
                __instance.SetCoolDown(role.TimeRemaining, CustomGameOptions.PoisonDuration);
                Utils.Rpc(CustomRPC.Poison, PlayerControl.LocalPlayer.PlayerId, target.PlayerId);
            }
            if (interact[0] == true)
            {
                role.Cooldown = CustomGameOptions.PoisonCD;
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
    }
}
