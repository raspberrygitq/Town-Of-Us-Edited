using HarmonyLib;
using TownOfUsEdited.Roles;
using TownOfUsEdited.Modifiers.AssassinMod;
using Reactor.Utilities;
using TownOfUsEdited.Patches;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace TownOfUsEdited.CrewmateRoles.JailorMod
{
    [HarmonyPatch(typeof(PlayerVoteArea))]
    public class AllowExtraVotes
    {
        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.VoteForMe))]
        public static class VoteForMe
        {
            
            public static bool Prefix(PlayerVoteArea __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jailor)) return true;
                var role = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
                if (__instance.Parent.state == MeetingHud.VoteStates.Proceeding ||
                    __instance.Parent.state == MeetingHud.VoteStates.Results)
                    return false;

                if (__instance == role.Jailed)
                {
                    if (!role.CanJail) return false;
                    if (role.JailedPlayer != null && !role.JailedPlayer.IsBlessed())
                    {
                        if (!role.JailedPlayer.Is(RoleEnum.Pestilence))
                        {
                            AssassinKill.MurderPlayer(role.JailedPlayer, PlayerControl.LocalPlayer);
                            Utils.Rpc(CustomRPC.Execution, role.JailedPlayer.PlayerId, PlayerControl.LocalPlayer.PlayerId);
                        }
                        else
                        {
                            AssassinKill.MurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                            Utils.Rpc(CustomRPC.Execution, PlayerControl.LocalPlayer.PlayerId, PlayerControl.LocalPlayer.PlayerId);
                        }
                        if (role.JailedPlayer.Is(Faction.Crewmates) && !PlayerControl.LocalPlayer.Is(Faction.Madmates) && !role.Player.Data.IsDead)
                        {
                            role.CanJail = false;
                            role.IncorrectKills++;
                            Coroutines.Start(Utils.FlashCoroutine(Color.red));
                            if (CustomGameOptions.JailorDies && !role.JailedPlayer.Is(RoleEnum.Pestilence))
                            {
                                AssassinKill.MurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                                Utils.Rpc(CustomRPC.Execution, PlayerControl.LocalPlayer.PlayerId, PlayerControl.LocalPlayer.PlayerId);
                            }
                        }
                        else if (PlayerControl.LocalPlayer.Is(Faction.Madmates) && !role.Player.Data.IsDead)
                        {
                            role.CanJail = false;
                            role.Kills++;
                        }
                        else if (!role.Player.Data.IsDead)
                        {
                            role.CorrectKills++;
                            Coroutines.Start(Utils.FlashCoroutine(Color.green));
                        }
                        var jailedPlayerRole = Role.GetRole(role.JailedPlayer);
                        jailedPlayerRole.DeathReason = DeathReasons.Executed;
                        Utils.Rpc(CustomRPC.SetDeathReason, role.JailedPlayer.PlayerId, (byte)DeathReasons.Executed);
                        role.JailedPlayer = null;
                        role.JailCell.Destroy();
                        role.Jailed.gameObject.SetActive(false);
                    }
                    else if (role.JailedPlayer.IsBlessed())
                    {
                        Coroutines.Start(Utils.FlashCoroutine(Colors.Oracle));
                        foreach (var oracle in role.JailedPlayer.GetOracle())
                        {
                            Utils.Rpc(CustomRPC.Bless, oracle.Player.PlayerId, (byte)2);
                        }
                    }
                    return false;
                }
                else return true;
            }
        }
    }
}