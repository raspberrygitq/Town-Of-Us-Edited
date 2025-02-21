using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.ImpostorRoles.HypnotistMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Hypnotist)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Hypnotist>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;
            if (__instance == role.HypnotiseButton)
            {
                if (role.Player.inVent) return false;
                if (!__instance.isActiveAndEnabled || role.ClosestPlayer == null) return false;
                if (__instance.isCoolingDown) return false;
                if (role.Cooldown > 0) return false;

                var interact = Utils.Interact(PlayerControl.LocalPlayer, target);
                if (interact[4] == true)
                {
                    role.HypnotisedPlayers.Add(target.PlayerId);
                    Utils.Rpc(CustomRPC.Hypnotise, PlayerControl.LocalPlayer.PlayerId, (byte)0, target.PlayerId);
                }
                if (interact[0] == true)
                {
                    role.Cooldown = CustomGameOptions.HypnotiseCd;
                    return false;
                }
                else if (interact[1] == true)
                {
                    role.Cooldown = CustomGameOptions.ProtectKCReset;
                    return false;
                }
                else if (interact[3] == true) return false;
                return false;
            }
            return true;
        }
    }
}