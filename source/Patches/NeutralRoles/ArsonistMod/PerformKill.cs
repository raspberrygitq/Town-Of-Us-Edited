using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.NeutralRoles.ArsonistMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Arsonist>(PlayerControl.LocalPlayer);
            if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return false;

            if (__instance == role.IgniteButton && role.DousedAlive > 0)
            {
                if (role.Cooldown <= 0 || (role.LastKiller && CustomGameOptions.IgniteCdRemoved))
                {
                    if (role.ClosestPlayerIgnite == null) return false;
                    var distBetweenPlayers2 = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayerIgnite);
                    var flag3 = distBetweenPlayers2 <
                                LegacyGameOptions.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
                    if (!flag3) return false;
                    if (!role.DousedPlayers.Contains(role.ClosestPlayerIgnite.PlayerId)) return false;

                    if (PlayerControl.LocalPlayer.IsControlled() && role.ClosestPlayerIgnite.Is(Faction.Coven))
                    {
                        Utils.Interact(role.ClosestPlayerIgnite, PlayerControl.LocalPlayer, true);
                        return false;
                    }
                    var interact2 = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayerIgnite);
                    if (interact2[4] == true) role.Ignite();
                    if (interact2[0] == true)
                    {
                        role.Cooldown = CustomGameOptions.DouseCd;
                        return false;
                    }
                    else if (interact2[1] == true)
                    {
                        role.Cooldown = CustomGameOptions.TempSaveCdReset;
                        return false;
                    }
                    else if (interact2[3] == true) return false;
                    return false;
                }
                else return false;
            }

            if (__instance != HudManager.Instance.KillButton) return true;
            if (role.DousedAlive == CustomGameOptions.MaxDoused) return false;
            if (role.ClosestPlayerDouse == null) return false;
            if (role.Cooldown > 0f) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayerDouse);
            var flag2 = distBetweenPlayers <
                        LegacyGameOptions.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag2) return false;
            if (role.DousedPlayers.Contains(role.ClosestPlayerDouse.PlayerId)) return false;
            if (PlayerControl.LocalPlayer.IsControlled() && role.ClosestPlayerDouse.Is(Faction.Coven))
            {
                Utils.Interact(role.ClosestPlayerDouse, PlayerControl.LocalPlayer, true);
                return false;
            }
            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayerDouse);
            if (interact[4] == true) role.DousedPlayers.Add(role.ClosestPlayerDouse.PlayerId);
            if (interact[0] == true)
            {
                role.Cooldown = CustomGameOptions.DouseCd;
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
