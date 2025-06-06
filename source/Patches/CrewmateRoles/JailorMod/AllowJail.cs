using HarmonyLib;
using TownOfUsEdited.Roles;
using TownOfUsEdited.Modifiers.AssassinMod;
using Reactor.Utilities;
using TownOfUsEdited.Patches;
using Reactor.Utilities.Extensions;

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
                        if ((role.JailedPlayer.Is(Faction.Crewmates) || (role.JailedPlayer.Is(Faction.NeutralBenign) && !CustomGameOptions.CanJailNB)
                        || (role.JailedPlayer.Is(Faction.NeutralKilling) && !CustomGameOptions.CanJailNK)
                        || (role.JailedPlayer.Is(Faction.Coven) && !CustomGameOptions.CanJailCoven)
                        || (role.JailedPlayer.Is(Faction.NeutralEvil) && !CustomGameOptions.CanJailNE)
                        || (role.JailedPlayer.Is(Faction.Madmates) && !CustomGameOptions.CanJailMad)) && !PlayerControl.LocalPlayer.Is(Faction.Madmates)
                        && !role.Player.Data.IsDead)
                        {
                            role.CanJail = false;
                            role.IncorrectKills++;
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