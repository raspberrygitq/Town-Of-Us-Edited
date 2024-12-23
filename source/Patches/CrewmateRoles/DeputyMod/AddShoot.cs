using HarmonyLib;
using TMPro;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.DeputyMod
{
    public class AddShoot
    {
        public static void UpdateButton(Deputy role, MeetingHud __instance)
        {
            var skip = __instance.SkipVoteButton;
            role.Shoot.gameObject.SetActive(skip.gameObject.active && !role.UsedShoot);
            role.Shoot.voteComplete = skip.voteComplete;
            role.Shoot.GetComponent<SpriteRenderer>().enabled = skip.GetComponent<SpriteRenderer>().enabled;
            role.Shoot.GetComponent<SpriteRenderer>().color = role.Color;
            role.Shoot.GetComponentsInChildren<TextMeshPro>()[0].text = "Shoot";
        }


        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public class MeetingHudStart
        {
            public static void GenButton(Deputy role, MeetingHud __instance)
            {
                var skip = __instance.SkipVoteButton;
                role.Shoot = Object.Instantiate(skip, skip.transform.parent);
                role.Shoot.Parent = __instance;
                role.Shoot.SetTargetPlayerId(251);
                role.Shoot.transform.localPosition = skip.transform.localPosition +
                                                       new Vector3(0f, -0.17f, 0f);
                skip.transform.localPosition += new Vector3(0f, 0.20f, 0f);
                UpdateButton(role, __instance);
            }

            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Deputy)) return;
                var deputyRole = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
                if (deputyRole.UsedShoot) return;
                GenButton(deputyRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ClearVote))]
        public class MeetingHudClearVote
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Deputy)) return;
                var deputyRole = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
                if (deputyRole.UsedShoot) return;
                UpdateButton(deputyRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
        public class MeetingHudConfirm
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Deputy)) return;
                var deputyRole = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
                if (deputyRole.UsedShoot) return;
                UpdateButton(deputyRole, __instance);
                deputyRole.Shoot.ClearButtons();
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Select))]
        public class MeetingHudSelect
        {
            public static void Postfix(MeetingHud __instance, int __0)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Deputy)) return;
                var deputyRole = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
                if (deputyRole.UsedShoot) return;
                if (__0 != 251) deputyRole.Shoot.ClearButtons();

                UpdateButton(deputyRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        public class MeetingHudVotingComplete
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Deputy)) return;
                var deputyRole = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
                if (deputyRole.UsedShoot) return;
                UpdateButton(deputyRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        public class MeetingHudUpdate
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Deputy)) return;
                var deputyRole = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
                if (deputyRole.UsedShoot) return;
                switch (__instance.state)
                {
                    case MeetingHud.VoteStates.Discussion:
                        if (__instance.discussionTimer < GameOptionsManager.Instance.currentNormalGameOptions.DiscussionTime)
                        {
                            deputyRole.Shoot.SetDisabled();
                            break;
                        }


                        deputyRole.Shoot.SetEnabled();
                        break;
                }

                UpdateButton(deputyRole, __instance);
            }
        }
    }
}