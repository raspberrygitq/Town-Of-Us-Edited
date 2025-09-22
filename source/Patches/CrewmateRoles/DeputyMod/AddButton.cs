using System;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUsEdited.CrewmateRoles.ImitatorMod;
using TownOfUsEdited.CrewmateRoles.MedicMod;
using TownOfUsEdited.CrewmateRoles.SwapperMod;
using TownOfUsEdited.CrewmateRoles.VigilanteMod;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.ImpostorRoles.BlackmailerMod;
using TownOfUsEdited.Modifiers.AssassinMod;
using TownOfUsEdited.NeutralRoles.DoomsayerMod;
using TownOfUsEdited.Patches;
using TownOfUsEdited.Roles;
using TownOfUsEdited.Roles.Modifiers;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Assassin = TownOfUsEdited.Roles.Modifiers.Assassin;
using TownOfUsEdited.CovenRoles.RitualistMod;
using Reactor.Utilities;
using System.Collections.Generic;

namespace TownOfUsEdited.CrewmateRoles.DeputyMod
{
    public class AddButtonDeputy
    {
        private static Sprite ShootSprite => TownOfUsEdited.ShootSprite;

        private static bool IsExempt(PlayerVoteArea voteArea)
        {
            if (voteArea.AmDead) return true;
            var player = Utils.PlayerById(voteArea.TargetPlayerId);
            if (player.IsJailed()) return true;
            if (PlayerControl.LocalPlayer == player) return true;
            if (PlayerControl.LocalPlayer.IsLover())
            {
                var lover = Modifier.GetModifier<Lover>(PlayerControl.LocalPlayer);
                if (lover.OtherLover.Player == player) return true;
            }
            if (
                    player == null ||
                    player.Data.IsDead ||
                    player.Data.Disconnected
                ) return true;
            return false;
        }

        public static void GenButton(Deputy role, PlayerVoteArea voteArea)
        {
            var targetId = voteArea.TargetPlayerId;
            if (IsExempt(voteArea))
            {
                role.Buttons[targetId] = null;
                return;
            }

            var confirmButton = voteArea.Buttons.transform.GetChild(0).gameObject;

            var newButton = Object.Instantiate(confirmButton, voteArea.transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            renderer.sprite = ShootSprite;
            newButton.transform.position = confirmButton.transform.position - new Vector3(0.75f, 0f, 0f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(Shoot(role, voteArea));
            role.Buttons[targetId] = newButton;
        }


        private static Action Shoot(Deputy role, PlayerVoteArea voteArea)
        {
            void Listener()
            {
                var target = Utils.PlayerById(voteArea.TargetPlayerId);
                if (target == role.Killer && !target.Is(RoleEnum.Pestilence) && !target.IsBlessed())
                {
                    Shoot(role, target);
                    if (target.Is(Faction.Crewmates)) role.IncorrectKills += 1;
                    else role.CorrectKills += 1;
                }
                else if (target == role.Killer && target.IsBlessed())
                {
                    Coroutines.Start(Utils.FlashCoroutine(Colors.Oracle));
                    foreach (var oracle in target.GetOracle())
                    {
                        Utils.Rpc(CustomRPC.Bless, oracle.Player.PlayerId, (byte)2);
                    }
                }
                else HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "You missed your shot! They are either not the killer or are invincible (Pestilence).");
                Utils.Rpc(CustomRPC.Camp, role.Player.PlayerId, (byte)2, target.PlayerId);
                role.Killer = null;
                RemoveButtons.HideButtons(role);
            }

            return Listener;
        }

        public static void Shoot(Deputy deputy, PlayerControl player, bool checkLover = true)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );

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
                    var buttons = Role.GetRole<Swapper>(player).Buttons;
                    foreach (var button in buttons)
                    {
                        if (button != null)
                        {
                            button.SetActive(false);
                            button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                        }
                    }
                    swapper.ListOfActives.Clear();
                    swapper.Buttons.Clear();
                    SwapVotes.Swap1 = null;
                    SwapVotes.Swap2 = null;
                    Utils.Rpc(CustomRPC.SetSwaps, sbyte.MaxValue, sbyte.MaxValue);
                }

                if (player.Is(RoleEnum.Imitator))
                {
                    var imitator = Role.GetRole<Imitator>(PlayerControl.LocalPlayer);
                    var buttons = Role.GetRole<Imitator>(player).Buttons;
                    foreach (var button in buttons)
                    {
                        if (button != null)
                        {
                            button.SetActive(false);
                            button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                        }
                    }
                    imitator.ListOfActives.Clear();
                    imitator.Buttons.Clear();
                    SetImitate.Imitate = null;
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
                    var rit = Role.GetRole<Ritualist>(PlayerControl.LocalPlayer);
                    ShowHideButtonsRitualist.HideButtons(rit);
                }

                if (player.Is(RoleEnum.Doomsayer))
                {
                    var doomsayer = Role.GetRole<Doomsayer>(PlayerControl.LocalPlayer);
                    ShowHideButtonsDoom.HideButtonsDoom(doomsayer);
                }

                if (player.Is(RoleEnum.Deputy))
                {
                    var dep = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
                    RemoveButtons.HideButtons(dep);
                }

                if (player.Is(RoleEnum.Politician))
                {
                    var politician = Role.GetRole<Politician>(PlayerControl.LocalPlayer);
                    politician.RevealButton.Destroy();
                }

                if (player.Is(RoleEnum.Mayor))
                {
                    var mayor = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
                    mayor.RevealButton.Destroy();
                }

                if (player.Is(RoleEnum.Knight))
                {
                    var knight = Role.GetRole<Knight>(PlayerControl.LocalPlayer);
                    knight.UsesText.Destroy();
                }

                if (player.Is(RoleEnum.Doctor))
                {
                    var doc = Role.GetRole<Doctor>(PlayerControl.LocalPlayer);
                    doc.UsesText.Destroy();
                }

                if (player.Is(RoleEnum.Hypnotist))
                {
                    var hypnotist = Role.GetRole<Hypnotist>(PlayerControl.LocalPlayer);
                    hypnotist.HysteriaButton.Destroy();
                }
            }
            player.Die(DeathReason.Kill, false);
            if (checkLover && player.IsLover() && CustomGameOptions.BothLoversDie)
            {
                var otherLover = Modifier.GetModifier<Lover>(player).OtherLover.Player;
                if (!otherLover.Is(RoleEnum.Pestilence)) Shoot(deputy, otherLover, false);
            }

            if (checkLover == true)
            {
                var playerRole = Role.GetRole(player);
                playerRole.DeathReason = DeathReasons.Executed;
                Utils.Rpc(CustomRPC.SetDeathReason, player.PlayerId, (byte)DeathReasons.Executed);
            }
            else
            {
                var playerRole = Role.GetRole(player);
                playerRole.DeathReason = DeathReasons.Suicided;
                Utils.Rpc(CustomRPC.SetDeathReason, player.PlayerId, (byte)DeathReasons.Suicided);
            }

            var deadPlayer = new DeadPlayer
            {
                PlayerId = player.PlayerId,
                KillerId = player.PlayerId,
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
                if (MeetingHud.Instance.state != MeetingHud.VoteStates.Results && MeetingHud.Instance.state != MeetingHud.VoteStates.Proceeding)
                {
                    AddButtonImitator.GenButton(imitatorRole, voteArea, true);
                }
            }

            if (AmongUsClient.Instance.AmHost) meetingHud.CheckForEndVoting();

            AddHauntPatch.AssassinatedPlayers.Add(player);
        }

        public static void AddDepButtons(MeetingHud __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Deputy))
            {
                var deputy = (Deputy)role;
                deputy.Buttons.Clear();
            }

            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Deputy)) return;
            if (PlayerControl.LocalPlayer.IsJailed()) return;
            var deputyrole = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
            if (deputyrole.Killer == null) return;
            foreach (var voteArea in __instance.playerStates)
            {
                GenButton(deputyrole, voteArea);
            }
        }
    }
}