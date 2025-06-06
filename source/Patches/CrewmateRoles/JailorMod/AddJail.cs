using HarmonyLib;
using Reactor.Utilities.Extensions;
using TMPro;
using TownOfUsEdited.Roles;
using UnityEngine;
using UnityEngine.UI;

namespace TownOfUsEdited.CrewmateRoles.JailorMod
{
    public class AddJail
    {
        public static void UpdateButton(Jailor role, MeetingHud __instance)
        {
            var skip = __instance.SkipVoteButton;
            role.Jailed.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !skip.voteComplete && !role.Player.IsJailed() && role.CanJail
            && role.JailedPlayer != null);
            role.Jailed.voteComplete = skip.voteComplete;
            role.Jailed.GetComponent<SpriteRenderer>().enabled = skip.GetComponent<SpriteRenderer>().enabled;
            role.Jailed.GetComponent<SpriteRenderer>().color = Patches.Colors.Jailor;
            role.Jailed.GetComponentsInChildren<TextMeshPro>()[0].text = "Execute";
        }

        public class AddJailButtons
        {
            public static void AddJailorButtons(MeetingHud __instance)
            {
                foreach (var role in Role.GetRoles(RoleEnum.Jailor))
                {
                    var jailor = (Jailor)role;
                    jailor.JailCell.Destroy();
                    if (jailor.JailedPlayer != null && !jailor.JailedPlayer.Data.IsDead && !jailor.JailedPlayer.Data.Disconnected
                    && !jailor.Player.Data.IsDead && !jailor.Player.Data.Disconnected)
                    {
                        foreach (var voteArea in __instance.playerStates)
                        {
                            if (jailor.JailedPlayer.PlayerId == voteArea.TargetPlayerId)
                            {
                                GenCell(jailor, voteArea);
                            }
                        }
                        if (jailor.Player == PlayerControl.LocalPlayer && jailor.CanJail && !jailor.Player.IsJailed())
                        {
                            var skip = __instance.SkipVoteButton;
                            jailor.Jailed = Object.Instantiate(skip, skip.transform.parent);
                            jailor.Jailed.Parent = __instance;
                            jailor.Jailed.SetTargetPlayerId(251);
                            jailor.Jailed.transform.localPosition = skip.transform.localPosition +
                                                                new Vector3(0f, -0.17f, 0f);
                            skip.transform.localPosition += new Vector3(0f, 0.20f, 0f);
                            UpdateButton(jailor, __instance);
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ClearVote))]
        public class MeetingHudClearVote
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jailor)) return;
                var jailRole = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
                if (jailRole.Jailed == null) return;
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
                if (jailRole.Jailed == null) return;
                jailRole.Jailed.gameObject.SetActive(false);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        public class MeetingHudVotingComplete
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jailor)) return;
                var jailRole = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
                if (jailRole.Jailed == null) return;
                UpdateButton(jailRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
        public class MeetingHudClose
        {
            public static void Postfix(MeetingHud __instance)
            {
                foreach (var role in Role.GetRoles(RoleEnum.Jailor))
                {
                    var jailor = (Jailor)role;
                    jailor.JailedPlayer = null;
                }
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        public class MeetingHudUpdate
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jailor)) return;
                var jailRole = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
                if (!jailRole.CanJail) return;
                if (jailRole.Jailed == null) return;
                switch (__instance.state)
                {
                    case MeetingHud.VoteStates.Discussion:
                        if (__instance.discussionTimer < GameOptionsManager.Instance.currentNormalGameOptions.DiscussionTime)
                        {
                            jailRole.Jailed.SetDisabled();
                            break;
                        }

                        jailRole.Jailed.SetEnabled();
                        break;
                }

                UpdateButton(jailRole, __instance);
            }
        }
        public static Sprite CellSprite => TownOfUsEdited.InJailSprite;

        public static void GenCell(Jailor role, PlayerVoteArea voteArea)
        {
            var confirmButton = voteArea.Buttons.transform.GetChild(0).gameObject;
            var parent = confirmButton.transform.parent.parent;

            var jailCell = Object.Instantiate(confirmButton, voteArea.transform);
            var cellRenderer = jailCell.GetComponent<SpriteRenderer>();
            var passive = jailCell.GetComponent<PassiveButton>();
            cellRenderer.sprite = CellSprite;
            jailCell.transform.localPosition = new Vector3(-0.95f, 0f, -2f);
            jailCell.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            jailCell.layer = 5;
            jailCell.transform.parent = parent;
            jailCell.transform.GetChild(0).gameObject.Destroy();

            passive.OnClick = new Button.ButtonClickedEvent();
            role.JailCell = jailCell;
        }
    }
}