using AmongUs.GameOptions;
using HarmonyLib;
using Reactor.Utilities;
using TownOfUsEdited.CrewmateRoles.ClericMod;
using TownOfUsEdited.CrewmateRoles.MedicMod;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Patches;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CrewmateRoles.SheriffMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public static class Kill
    {
        [HarmonyPriority(Priority.First)]
        private static bool Prefix(KillButton __instance)
        {
            if (__instance != HudManager.Instance.KillButton) return true;
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

            var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
            if (!abilityUsed) return false;
            if (role.ClosestPlayer.Is(RoleEnum.Pestilence))
            {
                Utils.RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                return false;
            }
            if (role.ClosestPlayer.IsInfected() || role.Player.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayer, role.Player);
            }
            if (role.ClosestPlayer.IsFortified())
            {
                Coroutines.Start(Utils.FlashCoroutine(Colors.Warden));
                foreach (var warden in role.ClosestPlayer.GetWarden())
                {
                    Utils.Rpc(CustomRPC.Fortify, (byte)1, warden.Player.PlayerId);
                }
                return false;
            }
            else if (role.ClosestPlayer == ShowShield.FirstRoundShielded) return false;
            else if (role.ClosestPlayer.IsOnAlert())
            {
                if (role.ClosestPlayer.IsShielded())
                {
                    foreach (var medic in role.ClosestPlayer.GetMedic())
                    {
                        Utils.Rpc(CustomRPC.AttemptSound, medic.Player.PlayerId, role.ClosestPlayer.PlayerId);
                        StopKill.BreakShield(medic.Player.PlayerId, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);
                    }

                    if (CustomGameOptions.ShieldBreaks) role.Cooldown = CustomGameOptions.SheriffKillCd;

                    Coroutines.Start(Utils.FlashCoroutine(new Color(0f, 0.5f, 0f, 1f)));

                    if (!PlayerControl.LocalPlayer.IsProtected() && !PlayerControl.LocalPlayer.IsBarriered())
                    {
                        Utils.RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                    }
                    else if (role.SheriffKillTimer() <= 0f)
                    {
                        if (PlayerControl.LocalPlayer.IsBarriered())
                        {
                            foreach (var cleric in PlayerControl.LocalPlayer.GetCleric())
                            {
                                StopAttack.NotifyCleric(cleric.Player.PlayerId, false);
                            }
                        }
                        role.Cooldown = CustomGameOptions.SheriffKillCd;
                        role.Cooldown = CustomGameOptions.TempSaveCdReset;
                    }
                }
                else if (role.Player.IsShielded())
                {
                    foreach (var medic in role.Player.GetMedic())
                    {
                        Utils.Rpc(CustomRPC.AttemptSound, medic.Player.PlayerId, role.Player.PlayerId);
                        StopKill.BreakShield(medic.Player.PlayerId, role.Player.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                    Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                    role.Cooldown = CustomGameOptions.SheriffKillCd;
                    if (CustomGameOptions.SheriffKillOther && !role.ClosestPlayer.IsProtected() && !role.ClosestPlayer.IsBarriered() && CustomGameOptions.KilledOnAlert)
                        Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                    if (role.ClosestPlayer.IsBarriered())
                    {
                        foreach (var cleric in role.ClosestPlayer.GetCleric())
                        {
                            StopAttack.NotifyCleric(cleric.Player.PlayerId);
                        }
                    }
                }
                else
                {
                    if (!PlayerControl.LocalPlayer.IsProtected() && !PlayerControl.LocalPlayer.IsBarriered())
                    {
                        Utils.RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                    }
                    else
                    {
                        if (PlayerControl.LocalPlayer.IsBarriered())
                        {
                            foreach (var cleric in PlayerControl.LocalPlayer.GetCleric())
                            {
                                StopAttack.NotifyCleric(cleric.Player.PlayerId, false);
                            }
                        }
                        Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                    }
                    if (CustomGameOptions.SheriffKillOther && !role.ClosestPlayer.IsProtected() && !role.ClosestPlayer.IsBarriered() && CustomGameOptions.KilledOnAlert)
                    {
                        Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                    }
                    else if (role.ClosestPlayer.IsBarriered())
                    {
                        foreach (var cleric in role.ClosestPlayer.GetCleric())
                        {
                            StopAttack.NotifyCleric(cleric.Player.PlayerId);
                        }
                    }
                    role.Cooldown = CustomGameOptions.SheriffKillCd;
                }

                return false;
            }
            else if (role.ClosestPlayer.IsShielded())
            {
                foreach (var medic in role.ClosestPlayer.GetMedic())
                {
                    Utils.Rpc(CustomRPC.AttemptSound, medic.Player.PlayerId, role.ClosestPlayer.PlayerId);
                    StopKill.BreakShield(medic.Player.PlayerId, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);
                }

                if (CustomGameOptions.ShieldBreaks) role.Cooldown = CustomGameOptions.SheriffKillCd;

                Coroutines.Start(Utils.FlashCoroutine(new Color(0f, 0.5f, 0f, 1f)));

                return false;
            }
            else if (role.ClosestPlayer.IsVesting())
            {
                Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);

                return false;
            }
            else if (role.ClosestPlayer.IsProtected() || role.ClosestPlayer.IsBarriered())
            {
                role.Cooldown = CustomGameOptions.SheriffKillCd;
                if (!flag4) Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                else
                {
                    if (role.ClosestPlayer.IsBarriered())
                    {
                        foreach (var cleric in role.ClosestPlayer.GetCleric())
                        {
                            StopAttack.NotifyCleric(cleric.Player.PlayerId);
                        }
                    }
                    role.Cooldown = CustomGameOptions.TempSaveCdReset;
                }
                return false;
            }

            if (!flag4 && !PlayerControl.LocalPlayer.Is(Faction.Madmates))
            {
                if (CustomGameOptions.SheriffKillOther)
                    Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                role.DeathReason = DeathReasons.Misfired;
                Utils.Rpc(CustomRPC.SetDeathReason, PlayerControl.LocalPlayer.PlayerId, (byte)DeathReasons.Misfired);
                role.Cooldown = CustomGameOptions.SheriffKillCd;
            }
            else if (flag4 && !PlayerControl.LocalPlayer.Is(Faction.Madmates))
            {
                Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                role.Cooldown = CustomGameOptions.SheriffKillCd;
            }
            else if (PlayerControl.LocalPlayer.Is(Faction.Madmates))
            {
                Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                role.Cooldown = CustomGameOptions.SheriffKillCd;
            }
            return false;
        }
    }
}
