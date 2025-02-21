using HarmonyLib;
using TownOfUsEdited.Roles;
using System.Linq;

namespace TownOfUsEdited.CovenRoles.VoodooMasterModMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    class MeetingUpdatePatch
    {
        public static void Postfix(MeetingHud __instance)
        {
            var vms = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.VoodooMaster)).ToList();
            foreach (var vm in vms)
            {
                var role = Role.GetRole<VoodooMaster>(vm);
                if (role.VoodooPlayer != null && role.VoodooPlayer == PlayerControl.LocalPlayer && !role.Player.Data.IsDead
                && !role.Player.Data.Disconnected && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    __instance.SkipVoteButton.gameObject.SetActive(false);
                    foreach (var votearea in __instance.playerStates)
                    {
                        votearea.Buttons.SetActive(false);
                    }
		            if (__instance.state == MeetingHud.VoteStates.NotVoted)
		            {
                        if (__instance.lastSecond == 1 && role.Voted && !MeetingHud.Instance.DidVote(PlayerControl.LocalPlayer.PlayerId))
                        {
                            __instance.CmdCastVote(PlayerControl.LocalPlayer.PlayerId, role.VotedFor);
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CastVote))]
    class MeetingCastVotePatch
    {
        public static void Postfix(MeetingHud __instance, [HarmonyArgument(0)] byte srcPlayerId, [HarmonyArgument(1)] byte suspectPlayerId)
        {
            var vms = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.VoodooMaster)).ToList();
            foreach (var vm in vms)
            {
                var role = Role.GetRole<VoodooMaster>(vm);
                if (role.Player.PlayerId == srcPlayerId)
                {
                    System.Console.WriteLine("Voodoo Voted!");
                    role.Voted = true;
                    role.VotedFor = suspectPlayerId;
                }
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    class MeetingStartPatch
    {
        public static void Postfix(MeetingHud __instance)
        {
            var vms = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.VoodooMaster)).ToList();
            foreach (var vm in vms)
            {
                var role = Role.GetRole<VoodooMaster>(vm);
                if (role.VoodooPlayer != null && role.VoodooPlayer == PlayerControl.LocalPlayer && !role.Player.Data.IsDead
                && !role.Player.Data.Disconnected && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    
                    var message = "A <color=#bf5fff>Voodoo Master</color> cast a spell on you, you can not vote and will be forced to vote for whoever the Voodoo Master votes.";
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message, false);
                }
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    class MeetingEndPatch
    {
        public static void Postfix()
        {
            var vms = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.VoodooMaster)).ToList();
            foreach (var vm in vms)
            {
                var role = Role.GetRole<VoodooMaster>(vm);
                if (role.VoodooPlayer != null) role.VoodooPlayer = null;
                if (role.Voted) role.Voted = false;
            }
        }
    }
}
