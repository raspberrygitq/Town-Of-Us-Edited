using HarmonyLib;
using TownOfUs.Modifiers.AssassinMod;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.DeputyMod
{
    public class AllowExtraVotes
    {
        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.VoteForMe))]
        public static class VoteForMe
        {
            public static bool Prefix(PlayerVoteArea __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Deputy)) return true;
                var role = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
                if (__instance.Parent.state == MeetingHud.VoteStates.Proceeding ||
                    __instance.Parent.state == MeetingHud.VoteStates.Results)
                    return false;

                if (__instance != role.Shoot)
                {
                    if (role.StartShooting)
                    {
                        var VotedPlayer = Utils.PlayerById(__instance.TargetPlayerId);
                        if (!VotedPlayer.Is(RoleEnum.Pestilence))
                        {
                            AssassinKill.MurderPlayer(VotedPlayer, role.Player);
                            Utils.Rpc(CustomRPC.Execution, VotedPlayer.PlayerId, role.Player.PlayerId);
                        }
                        else
                        {
                            AssassinKill.MurderPlayer(role.Player, role.Player);
                            Utils.Rpc(CustomRPC.Execution, role.Player.PlayerId, role.Player.PlayerId);
                        }
                        role.StartShooting = false;
                        MeetingHud.Instance.SkipVoteButton.gameObject.SetActive(true);
                        return false;
                    }
                    return true;
                }
                else
                {
                    role.StartShooting = true;
                    AddShoot.UpdateButton(role, MeetingHud.Instance);
                    MeetingHud.Instance.SkipVoteButton.gameObject.SetActive(false);
                    return false;
                }
            }
        }
    }
}