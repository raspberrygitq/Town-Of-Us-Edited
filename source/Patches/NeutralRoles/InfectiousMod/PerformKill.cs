using HarmonyLib;
using TownOfUsEdited.Roles;
using AmongUs.GameOptions;

namespace TownOfUsEdited.NeutralRoles.InfectiousMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Infectious);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Infectious>(PlayerControl.LocalPlayer);
            if (role.Player.inVent) return false;
            var killbutton = DestroyableSingleton<HudManager>.Instance.KillButton;

            if (__instance == role.InfectButton)
            {
                if (role.Cooldown > 0) return false;
                if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return false;
                if (role.ClosestPlayerInfect == null) return false;
                var interact1 = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayerInfect);
                if (interact1[4] == true)
                {
                    role.Infected.Add(role.ClosestPlayerInfect.PlayerId);
                    Utils.Rpc(CustomRPC.InfectiousInfect, role.ClosestPlayerInfect.PlayerId, PlayerControl.LocalPlayer.PlayerId);
                    role.Cooldown = CustomGameOptions.InfectiousCD;
                    return false;
                }
                if (interact1[0] == true)
                {
                    role.Cooldown = CustomGameOptions.InfectiousCD;
                    return false;
                }
                else if (interact1[1] == true)
                {
                    role.Cooldown = CustomGameOptions.TempSaveCdReset;
                    return false;
                }
                else if (interact1[3] == true) return false;
                return false;
            }
            else if (__instance == killbutton)
            {
                if (role.Cooldown > 0) return false;
                if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return false;
                if (role.ClosestPlayer == null) return false;
                if (PlayerControl.LocalPlayer.IsControlled() && role.ClosestPlayer.Is(Faction.Coven))
                {
                    Utils.Interact(role.ClosestPlayer, PlayerControl.LocalPlayer, true);
                    return false;
                }
                
                Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
            }
            return false;
        }
    }
}
