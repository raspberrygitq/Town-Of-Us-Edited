using HarmonyLib;
using TMPro;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using Assassin = TownOfUs.Roles.Modifiers.Assassin;

namespace TownOfUs.CrewmateRoles.JailorMod
{
    public class AddJail
    {
        public static void UpdateButton(Jailor role, MeetingHud __instance)
        {
            if (!role.CanJail) return;
            if (role.JailedPlayer == null || role.JailedPlayer.Data.IsDead || role.JailedPlayer.Data.Disconnected) return;
            var skip = __instance.SkipVoteButton;
            role.Jailed.gameObject.SetActive(skip.gameObject.active);
            role.Jailed.voteComplete = skip.voteComplete;
            role.Jailed.GetComponent<SpriteRenderer>().enabled = skip.GetComponent<SpriteRenderer>().enabled;
            role.Jailed.GetComponent<SpriteRenderer>().color = Patches.Colors.Jailor;
            role.Jailed.GetComponentsInChildren<TextMeshPro>()[0].text = "Execute";
        }


        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public class MeetingHudStart
        {
            public static void GenButton(Jailor role, MeetingHud __instance)
            {
                if (role.CanJail && role.JailedPlayer != null && !role.JailedPlayer.Data.IsDead && !role.JailedPlayer.Data.Disconnected)
                {
                    var skip = __instance.SkipVoteButton;
                    role.Jailed = Object.Instantiate(skip, skip.transform.parent);
                    role.Jailed.Parent = __instance;
                    role.Jailed.SetTargetPlayerId(251);
                    role.Jailed.transform.localPosition = skip.transform.localPosition +
                                                        new Vector3(0f, -0.17f, 0f);
                    skip.transform.localPosition += new Vector3(0f, 0.20f, 0f);
                    UpdateButton(role, __instance);
                }
            }

            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jailor)) return;
                var jailRole = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
                GenButton(jailRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ClearVote))]
        public class MeetingHudClearVote
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jailor)) return;
                var jailRole = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
                UpdateButton(jailRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
        public class MeetingHudConfirm
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jailor)) return;
                var jailRole = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
                jailRole.Jailed.ClearButtons();
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        public class MeetingHudVotingComplete
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jailor)) return;
                var jailRole = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
                UpdateButton(jailRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
        public class MeetingHudClose
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jailor)) return;
                var jailRole = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
                if (jailRole.JailedAssassin == true)
                {
                    new Assassin(jailRole.JailedPlayer);
                    Utils.Rpc(CustomRPC.SetJailorAssassin, jailRole.JailedPlayer.PlayerId);
                }
                jailRole.JailedPlayer = null;
                Utils.Rpc(CustomRPC.UpdateJail, PlayerControl.LocalPlayer.PlayerId);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        public class MeetingHudUpdate
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jailor)) return;
                var jailRole = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
                switch (__instance.state)
                {
                    case MeetingHud.VoteStates.Discussion:
                        if (__instance.discussionTimer < GameOptionsManager.Instance.currentNormalGameOptions.DiscussionTime)
                        {
                            if (!jailRole.CanJail) return;
                            jailRole.Jailed.SetDisabled();
                            break;
                        }

                        if (!jailRole.CanJail) return;
                        jailRole.Jailed.SetEnabled();
                        break;
                }

                UpdateButton(jailRole, __instance);
            }
        }
    }
}