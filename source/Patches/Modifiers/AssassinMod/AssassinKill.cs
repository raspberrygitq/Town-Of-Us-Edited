﻿using System.Linq;
using TownOfUsEdited.Roles;
using UnityEngine;
using UnityEngine.UI;
using TownOfUsEdited.CrewmateRoles.MedicMod;
using TownOfUsEdited.CrewmateRoles.SwapperMod;
using TownOfUsEdited.CrewmateRoles.VigilanteMod;
using TownOfUsEdited.NeutralRoles.DoomsayerMod;
using TownOfUsEdited.ImpostorRoles.BlackmailerMod;
using TownOfUsEdited.Roles.Modifiers;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.CrewmateRoles.ImitatorMod;
using TownOfUsEdited.Patches;
using Reactor.Utilities.Extensions;
using Assassin = TownOfUsEdited.Roles.Modifiers.Assassin;
using TownOfUsEdited.CovenRoles.RitualistMod;
using TownOfUsEdited.CrewmateRoles.DeputyMod;
using System.Collections.Generic;

namespace TownOfUsEdited.Modifiers.AssassinMod
{
    public class AssassinKill
    {
        public static void RpcMurderPlayer(PlayerControl player, PlayerControl assassin)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );
            RpcMurderPlayer(voteArea, player, assassin);
        }
        public static void RpcMurderPlayer(PlayerVoteArea voteArea, PlayerControl player, PlayerControl assassin)
        {
            MurderPlayer(voteArea, player, assassin);
            AssassinKillCount(player, assassin);
            Utils.Rpc(CustomRPC.AssassinKill, player.PlayerId, assassin.PlayerId);
        }

        public static void MurderPlayer(PlayerControl player, PlayerControl killer, bool checkLover = true)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );
            MurderPlayer(voteArea, player, killer, checkLover);
        }
        public static void AssassinKillCount(PlayerControl player, PlayerControl assassin)
        {
            var assassinPlayer = Role.GetRole(assassin);
            if (player == assassin) assassinPlayer.IncorrectAssassinKills += 1;
            else assassinPlayer.CorrectAssassinKills += 1;
        }
        public static void MurderPlayer(
            PlayerVoteArea voteArea,
            PlayerControl player,
            PlayerControl killer,
            bool checkLover = true
        )
        {
            var hudManager = DestroyableSingleton<HudManager>.Instance;
            if (checkLover)
            {
                SoundManager.Instance.PlaySound(player.KillSfx, false, 0.8f);
                hudManager.KillOverlay.ShowKillAnimation(player.Data, player.Data);
            }
            var amOwner = player.AmOwner;
            if (amOwner)
            {
                Utils.ShowDeadBodies = true;
                hudManager.ShadowQuad.gameObject.SetActive(false);
                player.nameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                player.RpcSetScanner(false);

                if (player.Is(RoleEnum.Swapper))
                {
                    var swapper = Role.GetRole<Swapper>(PlayerControl.LocalPlayer);
                    swapper.ListOfActives.Clear();
                    swapper.Buttons.Clear();
                    SwapVotes.Swap1 = null;
                    SwapVotes.Swap2 = null;
                    Utils.Rpc(CustomRPC.SetSwaps, sbyte.MaxValue, sbyte.MaxValue);
                    var buttons = Role.GetRole<Swapper>(player).Buttons;
                    foreach (var button in buttons)
                    {
                        button.SetActive(false);
                        button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                    }
                }

                if (player.Is(RoleEnum.Imitator))
                {
                    var imitator = Role.GetRole<Imitator>(PlayerControl.LocalPlayer);
                    imitator.ListOfActives.Clear();
                    imitator.Buttons.Clear();
                    SetImitate.Imitate = null;
                    var buttons = Role.GetRole<Imitator>(player).Buttons;
                    foreach (var button in buttons)
                    {
                        button.SetActive(false);
                        button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                    }
                }

                if (player.Is(RoleEnum.Vigilante))
                {
                    var retributionist = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer);
                    ShowHideButtonsVigi.HideButtonsVigi(retributionist);
                }

                if (player.Is(AbilityEnum.Assassin))
                {
                    var assassin = Ability.GetAbility<Assassin>(PlayerControl.LocalPlayer);
                    ShowHideButtons.HideButtons(assassin);
                }

                if (player.Is(RoleEnum.Ritualist))
                {
                    var ritualist = Role.GetRole<Ritualist>(PlayerControl.LocalPlayer);
                    ShowHideButtonsRitualist.HideButtons(ritualist);
                }

                if (player.Is(RoleEnum.Doomsayer))
                {
                    var doomsayer = Role.GetRole<Doomsayer>(PlayerControl.LocalPlayer);
                    ShowHideButtonsDoom.HideButtonsDoom(doomsayer);
                }

                if (player.Is(RoleEnum.Mayor))
                {
                    var mayor = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
                    mayor.RevealButton.Destroy();
                }

                if (player.Is(RoleEnum.Politician))
                {
                    var politician = Role.GetRole<Politician>(PlayerControl.LocalPlayer);
                    politician.RevealButton.Destroy();
                }

                if (player.Is(RoleEnum.Deputy))
                {
                    var dep = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
                    RemoveButtons.HideButtons(dep);
                }
            }
            player.Die(DeathReason.Kill, false);
            if (checkLover && player.IsLover() && CustomGameOptions.BothLoversDie)
            {
                var otherLover = Modifier.GetModifier<Lover>(player).OtherLover.Player;
                if (!otherLover.Is(RoleEnum.Pestilence)) MurderPlayer(otherLover, otherLover, false);
            }

            if (checkLover == true)
            {
                var playerRole = Role.GetRole(player);
                if (killer.Is(RoleEnum.Hunter) || killer.Is(RoleEnum.Jailor))
                {
                    playerRole.DeathReason = DeathReasons.Executed;
                    Utils.Rpc(CustomRPC.SetDeathReason, player.PlayerId, (byte)DeathReasons.Executed);
                }
                else if (killer.Is(RoleEnum.TalkativeWolf))
                {
                    playerRole.DeathReason = DeathReasons.Suicided;
                    Utils.Rpc(CustomRPC.SetDeathReason, player.PlayerId, (byte)DeathReasons.Suicided);
                }
                else
                {
                    playerRole.DeathReason = DeathReasons.Guessed;
                    Utils.Rpc(CustomRPC.SetDeathReason, player.PlayerId, (byte)DeathReasons.Guessed);
                }
            }
            else
            {
                var playerRole = Role.GetRole(player);
                playerRole.DeathReason = DeathReasons.Suicided;
            }

            var deadPlayer = new DeadPlayer
            {
                PlayerId = player.PlayerId,
                KillerId = killer.PlayerId,
                KillTime = System.DateTime.UtcNow,
            };

            Murder.KilledPlayers.Add(deadPlayer);
            if (voteArea == null) return;
            if (voteArea.DidVote) voteArea.UnsetVote();
            voteArea.AmDead = true;
            voteArea.Overlay.gameObject.SetActive(true);
            voteArea.Overlay.color = Color.white;
            voteArea.XMark.gameObject.SetActive(true);
            voteArea.XMark.transform.localScale = Vector3.one;

            var meetingHud = MeetingHud.Instance;
            if (amOwner)
            {
                meetingHud.SetForegroundForDead();
            }

            var blackmailers = Role.AllRoles.Where(x => x.RoleType == RoleEnum.Blackmailer && x.Player != null).Cast<Blackmailer>();
            var blackmailed = new List<PlayerControl>();
            foreach (var role in blackmailers)
            {
                if (role.Blackmailed != null && !blackmailed.Contains(role.Blackmailed))
                {
                    blackmailed.Add(role.Blackmailed);
                    if (voteArea.TargetPlayerId == role.Blackmailed.PlayerId)
                    {
                        if (BlackmailMeetingUpdate.PrevXMark != null && BlackmailMeetingUpdate.PrevOverlay != null)
                        {
                            voteArea.XMark.sprite = BlackmailMeetingUpdate.PrevXMark;
                            voteArea.Overlay.sprite = BlackmailMeetingUpdate.PrevOverlay;
                            voteArea.XMark.transform.localPosition = new Vector3(
                                voteArea.XMark.transform.localPosition.x - BlackmailMeetingUpdate.LetterXOffset,
                                voteArea.XMark.transform.localPosition.y - BlackmailMeetingUpdate.LetterYOffset,
                                voteArea.XMark.transform.localPosition.z);
                        }
                    }
                }
            }

            var jailors = Role.AllRoles.Where(x => x.RoleType == RoleEnum.Jailor && x.Player != null).Cast<Jailor>();
            foreach (var role in jailors)
            {
                if (role.JailedPlayer == player || role.Player == player)
                {
                    role.JailCell.Destroy();
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var vigi = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer);
                ShowHideButtonsVigi.HideTarget(vigi, voteArea.TargetPlayerId);
            }

            if (PlayerControl.LocalPlayer.Is(AbilityEnum.Assassin) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var assassin = Ability.GetAbility<Assassin>(PlayerControl.LocalPlayer);
                ShowHideButtons.HideTarget(assassin, voteArea.TargetPlayerId);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Ritualist) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var ritualist = Role.GetRole<Ritualist>(PlayerControl.LocalPlayer);
                ShowHideButtonsRitualist.HideTarget(ritualist, voteArea.TargetPlayerId);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var doom = Role.GetRole<Doomsayer>(PlayerControl.LocalPlayer);
                ShowHideButtonsDoom.HideTarget(doom, voteArea.TargetPlayerId);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Deputy) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var dep = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
                if (dep.Buttons.Count > 0 && dep.Buttons[voteArea.TargetPlayerId] != null)
                {
                    dep.Buttons[voteArea.TargetPlayerId].SetActive(false);
                    dep.Buttons[voteArea.TargetPlayerId].GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Swapper) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var swapper = Role.GetRole<Swapper>(PlayerControl.LocalPlayer);
                var index = int.MaxValue;
                for (var i = 0; i < swapper.ListOfActives.Count; i++)
                {
                    if (swapper.ListOfActives[i].Item1 == voteArea.TargetPlayerId)
                    {
                        index = i;
                        break;
                    }
                }
                if (index != int.MaxValue)
                {
                    var button = swapper.Buttons[index];
                    if (button != null)
                    {
                        if (button.GetComponent<SpriteRenderer>().sprite == TownOfUsEdited.SwapperSwitch)
                        {
                            swapper.ListOfActives[index] = (swapper.ListOfActives[index].Item1, false);
                            if (SwapVotes.Swap1 == voteArea) SwapVotes.Swap1 = null;
                            if (SwapVotes.Swap2 == voteArea) SwapVotes.Swap2 = null;
                            Utils.Rpc(CustomRPC.SetSwaps, sbyte.MaxValue, sbyte.MaxValue);
                        }
                        button.SetActive(false);
                        button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                        swapper.Buttons[index] = null;
                    }
                }
            }

            foreach (var playerVoteArea in meetingHud.playerStates)
            {
                if (playerVoteArea.VotedFor != player.PlayerId) continue;
                playerVoteArea.UnsetVote();
                var voteAreaPlayer = Utils.PlayerById(playerVoteArea.TargetPlayerId);
                if (voteAreaPlayer.Is(RoleEnum.Prosecutor))
                {
                    var pros = Role.GetRole<Prosecutor>(voteAreaPlayer);
                    pros.ProsecuteThisMeeting = false;
                }
                if (!voteAreaPlayer.AmOwner) continue;
                meetingHud.ClearVote();
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Imitator) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var imitatorRole = Role.GetRole<Imitator>(PlayerControl.LocalPlayer);
                if (!meetingHud.playerStates[PlayerControl.LocalPlayer.PlayerId].DidVote)
                if (MeetingHud.Instance.state != MeetingHud.VoteStates.Results && MeetingHud.Instance.state != MeetingHud.VoteStates.Proceeding)
                {
                    AddButtonImitator.GenButton(imitatorRole, voteArea, true);
                }
            }

            if (AmongUsClient.Instance.AmHost) meetingHud.CheckForEndVoting();

            AddHauntPatch.AssassinatedPlayers.Add(player);
        }
    }
}
