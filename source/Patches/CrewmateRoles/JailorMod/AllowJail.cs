using HarmonyLib;
using TownOfUs.Roles;
using TownOfUs.Modifiers.AssassinMod;

namespace TownOfUs.CrewmateRoles.JailorMod
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
                    if (role.JailedPlayer.Is(Faction.Crewmates) || (role.JailedPlayer.Is(Faction.NeutralBenign) && !CustomGameOptions.CanJailNB)
                    || (role.JailedPlayer.Is(Faction.NeutralKilling) && !CustomGameOptions.CanJailNK)
                    || (role.JailedPlayer.Is(Faction.Coven) && !CustomGameOptions.CanJailCoven)
                    || (role.JailedPlayer.Is(Faction.NeutralEvil) && !CustomGameOptions.CanJailNE)
                    || (role.JailedPlayer.Is(Faction.Madmates) && !CustomGameOptions.CanJailMad))
                    {
                        role.CanJail = false;
                        if (CustomGameOptions.JailorDies && !PlayerControl.LocalPlayer.Is(Faction.Madmates) && !role.JailedPlayer.Is(RoleEnum.Pestilence))
                        {
                            AssassinKill.MurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                            Utils.Rpc(CustomRPC.Execution, PlayerControl.LocalPlayer.PlayerId, PlayerControl.LocalPlayer.PlayerId);
                        }
                    }
                    if (PlayerControl.LocalPlayer.Is(Faction.Madmates))
                    {
                        role.CanJail = false;
                    }
                    if (!role.JailedPlayer.Is(RoleEnum.Pestilence))
                    {
                        AssassinKill.MurderPlayer(role.JailedPlayer, PlayerControl.LocalPlayer);
                        Utils.Rpc(CustomRPC.Execution, role.JailedPlayer.PlayerId, PlayerControl.LocalPlayer.PlayerId);
                    }
                    else
                    {
                        AssassinKill.MurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                        Utils.Rpc(CustomRPC.Execution,  PlayerControl.LocalPlayer.PlayerId, PlayerControl.LocalPlayer.PlayerId);
                    }
                    var jailedPlayerRole = Role.GetRole(role.JailedPlayer);
                    jailedPlayerRole.DeathReason = DeathReasons.Executed;
                    Utils.Rpc(CustomRPC.SetDeathReason, role.JailedPlayer.PlayerId, (byte)DeathReasons.Executed);
                    role.Jailed.gameObject.SetActive(false);
                    role.Jailed.ClearButtons();
                    return false;
                }
                else return true;
            }
        }
    }
}