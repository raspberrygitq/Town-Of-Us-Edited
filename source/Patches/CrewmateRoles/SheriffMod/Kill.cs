using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.SheriffMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public static class Kill
    {
        [HarmonyPriority(Priority.First)]
        private static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff);
            if (!flag) return true;
            var role = Role.GetRole<Sheriff>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (role.Cooldown > 0) return false;
            if (!__instance.enabled || role.ClosestPlayer == null) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers < LegacyGameOptions.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag3) return false;
            if (PlayerControl.LocalPlayer.IsControlled() && role.ClosestPlayer.Is(Faction.Coven))
            {
                Utils.Interact(role.ClosestPlayer, PlayerControl.LocalPlayer, true);
                return false;
            }

            var flag4 = role.ClosestPlayer.Data.IsImpostor() ||
                        role.ClosestPlayer.Is(Faction.NeutralKilling) && CustomGameOptions.SheriffKillsNK ||
                        role.ClosestPlayer.Is(Faction.Madmates) && CustomGameOptions.SheriffKillsMad ||
                        role.ClosestPlayer.Is(Faction.Coven) && CustomGameOptions.SheriffKillsCoven ||
                        role.ClosestPlayer.Is(Faction.NeutralEvil) && CustomGameOptions.SheriffKillsNE ||
                        role.ClosestPlayer.Is(Faction.NeutralBenign) && CustomGameOptions.SheriffKillsNB;

            if (!flag4 && !PlayerControl.LocalPlayer.Is(Faction.Madmates))
            {
                if (CustomGameOptions.SheriffKillOther)
                    Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
                Utils.Interact(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer, true);
                role.DeathReason = DeathReasons.Misfired;
                Utils.Rpc(CustomRPC.SetDeathReason, PlayerControl.LocalPlayer.PlayerId, (byte)DeathReasons.Misfired);
                role.Cooldown = CustomGameOptions.SheriffKillCd;
            }
            else if (flag4 && !PlayerControl.LocalPlayer.Is(Faction.Madmates))
            {
                Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
                role.Cooldown = CustomGameOptions.SheriffKillCd;
            }
            else if (PlayerControl.LocalPlayer.Is(Faction.Madmates))
            {
                Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
                role.Cooldown = CustomGameOptions.SheriffKillCd;
            }
            return false;
        }
    }
}
