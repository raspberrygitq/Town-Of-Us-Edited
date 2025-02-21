using System.Collections;
using HarmonyLib;
using Reactor.Utilities;
using TownOfUsEdited.CrewmateRoles.AltruistMod;
using TownOfUsEdited.Roles;
using TownOfUsEdited.Roles.Modifiers;
using UnityEngine;

namespace TownOfUsEdited.CrewmateRoles.ProsecutorMod
{
    public class AllowExtraVotes
    {
        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.VoteForMe))]
        public static class VoteForMe
        {
            public static bool Prefix(PlayerVoteArea __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Prosecutor)) return true;
                var role = Role.GetRole<Prosecutor>(PlayerControl.LocalPlayer);
                if (__instance.Parent.state == MeetingHud.VoteStates.Proceeding ||
                    __instance.Parent.state == MeetingHud.VoteStates.Results)
                    return false;

                if (__instance != role.Prosecute)
                {
                    if (role.StartProsecute)
                    {
                        if (MeetingHud.Instance.lastSecond < 10)
                        {
                            role.StartProsecute = false;
                            MeetingHud.Instance.SkipVoteButton.gameObject.SetActive(true);
                            return false;
                        }
                        else
                        {
                            role.ProsecuteThisMeeting = true;
                            role.StartProsecute = false;
                            var VotedPlayer = Utils.PlayerById(__instance.TargetPlayerId);
                            MeetingHud.Instance.SkipVoteButton.gameObject.SetActive(true);
                            Prosecute(role.Player, VotedPlayer);
                            Utils.Rpc(CustomRPC.Prosecute, role.Player.PlayerId, VotedPlayer.PlayerId);
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    role.StartProsecute = true;
                    MeetingHud.Instance.SkipVoteButton.gameObject.SetActive(false);
                    AddProsecute.UpdateButton(role, MeetingHud.Instance);
                    return false;
                }
            }
            
            public static void Prosecute(PlayerControl prosecutor, PlayerControl VotedPlayer)
            {
                var role = Role.GetRole<Prosecutor>(prosecutor);
                Coroutines.Start(ProsecuteCoroutine(MeetingHud.Instance, VotedPlayer));
            }

            public static IEnumerator ProsecuteCoroutine(MeetingHud __instance, PlayerControl VotedPlayer)
            {
                foreach (var role in Role.GetRoles(RoleEnum.Prosecutor))
                {
                    var prosecutor = (Prosecutor)role;
                    if (prosecutor.ProsecuteThisMeeting)
                    {
                        ConsoleJoystick.SetMode_Task();
		                yield return DestroyableSingleton<HudManager>.Instance.CoFadeFullScreen(Color.clear, Color.black, 1f, false);
		                ExileController exileController = Object.Instantiate<ExileController>(ShipStatus.Instance.ExileCutscenePrefab);
		                exileController.transform.SetParent(DestroyableSingleton<HudManager>.Instance.transform, false);
		                exileController.transform.localPosition = new Vector3(0f, 0f, -60f);
                        MeetingHud.Instance.gameObject.SetActive(false);
		                exileController.BeginForGameplay(VotedPlayer.Data, false);
                        yield return new WaitForSeconds(6);
                        var votedPlayerRole = Role.GetRole(VotedPlayer);
                        votedPlayerRole.DeathReason = DeathReasons.Exiled;
                        Utils.Rpc(CustomRPC.SetDeathReason, VotedPlayer.PlayerId, (byte)DeathReasons.Exiled);
		                __instance.DespawnOnDestroy = false;
		                if (MapBehaviour.Instance)
		                {
			                MapBehaviour.Instance.Close();
		                }
                        if (PlayerControl.LocalPlayer == VotedPlayer)
                        {
                            Utils.ShowDeadBodies = true;
                        }
                        MeetingHud.Instance.gameObject.SetActive(true);
                        prosecutor.Prosecuted = true;
                        if ((VotedPlayer.Is(Faction.Crewmates) && CustomGameOptions.ProsDiesOnIncorrectPros && !role.Player.Is(Faction.Madmates)) || (CustomGameOptions.GameMode == GameMode.Werewolf && VotedPlayer.Is(Faction.Crewmates)))
                        {
                            KillButtonTarget.DontRevive = prosecutor.Player.PlayerId;
                            prosecutor.Player.Exiled();
                            prosecutor.DeathReason = DeathReasons.Exiled;
                            Utils.Rpc(CustomRPC.SetDeathReason, prosecutor.Player.PlayerId, (byte)DeathReasons.Exiled);
                            if (PlayerControl.LocalPlayer == prosecutor.Player)
                            {
                                Utils.ShowDeadBodies = true;
                            }
                        }
                        if (VotedPlayer.IsLover() && CustomGameOptions.BothLoversDie)
                        {
                            var otherlover = Modifier.GetModifier<Lover>(VotedPlayer).OtherLover;
                            otherlover.Player.Exiled();
                            var otherloverRole = Role.GetRole(otherlover.Player);
                            otherloverRole.DeathReason = DeathReasons.Suicided;
                            Utils.Rpc(CustomRPC.SetDeathReason, otherlover.Player.PlayerId, (byte)DeathReasons.Suicided);
                            if (PlayerControl.LocalPlayer == otherlover.Player)
                            {
                                Utils.ShowDeadBodies = true;
                            }
                        }
                        yield return new WaitForSeconds(2);
                        prosecutor.ProsecuteThisMeeting = false;
		                yield break;
                    }
                }
            }
        }
    }
}