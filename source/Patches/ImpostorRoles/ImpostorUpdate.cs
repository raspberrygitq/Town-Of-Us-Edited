using System.Linq;
using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUs.CrewmateRoles.SheriffMod;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.ImpostorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class ImpostorUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            var player = PlayerControl.LocalPlayer;
            // Check if there is only one player or if local player is null or dead
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;

            var aliveimp = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Impostors) && !x.Data.IsDead).ToList();

            if (!MeetingHud.Instance && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started
            && (__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
            && CustomGameOptions.GameMode != GameMode.Chaos && CustomGameOptions.GameMode != GameMode.Werewolf
            && (!PlayerControl.LocalPlayer.Is(RoleEnum.Spirit) || Role.GetRole<Spirit>(PlayerControl.LocalPlayer).Caught)
            && (!PlayerControl.LocalPlayer.Is(RoleEnum.Mafioso) || aliveimp.Count <= 1)
            && PlayerControl.LocalPlayer.Data.IsImpostor())
            {
                __instance.SabotageButton.Show();
                // Sabotage DoClick patch located in PerformKill.cs in EngineerMod
            }

            foreach (var player2 in PlayerControl.AllPlayerControls)
            {
                // Add red color to impostors names
                if ((player2.Data.IsImpostor() || player2.Is(Faction.Madmates)) && (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(Faction.Madmates)))
                {
                    if (CustomGameOptions.GameMode == GameMode.Werewolf)
                    {
                        if (player2 != PlayerControl.LocalPlayer && !CustomGameOptions.ImpostorSeeRoles)
                        {
                            player2.nameText().color = Patches.Colors.Werewolf;
                        }
                    }
                    else player2.nameText().color = Patches.Colors.Impostor;
                }
            }

            if (HudManager.Instance?.Chat != null)
            {
                // Add red color to chat bubble
                foreach (var player3 in PlayerControl.AllPlayerControls)
                {
                    foreach (var bubble in HudManager.Instance.Chat.chatBubblePool.activeChildren)
                    {
                        if (bubble.Cast<ChatBubble>().NameText != null &&
                            player3.Data.PlayerName == bubble.Cast<ChatBubble>().NameText.text &&
                            (player3.Data.IsImpostor() || player3.Is(Faction.Madmates)) && (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(Faction.Madmates)))
                        {
                            if (CustomGameOptions.GameMode == GameMode.Werewolf)
                            {
                                bubble.Cast<ChatBubble>().NameText.color = Patches.Colors.Werewolf;
                            }
                            else bubble.Cast<ChatBubble>().NameText.color = Palette.ImpostorRed;
                        }
                    }
                }
            }

            if (MeetingHud.Instance) UpdateMeeting(MeetingHud.Instance);

            var impsalive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Impostors) && !x.Is(RoleEnum.Mafioso) && !x.Data.IsDead && !x.Data.Disconnected).ToList();

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Poisoner) || (PlayerControl.LocalPlayer.Is(RoleEnum.Mafioso) && impsalive.Count > 0)
            || !PlayerControl.LocalPlayer.Data.IsImpostor()) return;

            // Check if the game state allows the KillButton to be active
            bool isKillButtonActive = __instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled;
            isKillButtonActive = isKillButtonActive && !MeetingHud.Instance && !player.Data.IsDead;
            isKillButtonActive = isKillButtonActive && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started;

            // Set KillButton's visibility
            __instance.KillButton.gameObject.SetActive(isKillButtonActive);

            __instance.KillButton.gameObject.transform.localPosition = new Vector3(0f, 1f, 0f);

            // Set KillButton's cooldown
            __instance.KillButton.SetCoolDown(KillTimer(), GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);

            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return;
            HUDKill.ImpKillTarget(__instance.KillButton);
        }

        public static void UpdateMeeting(MeetingHud __instance)
        {
            // Add red color to impostors names in meeting
            var imps = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Impostors) || x.Is(Faction.Madmates)).ToList();
            foreach (PlayerVoteArea pva in __instance.playerStates)
            {
                if (imps.Any(x => x.PlayerId == pva.TargetPlayerId) && (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(Faction.Madmates)))
                {
                    if (CustomGameOptions.GameMode == GameMode.Werewolf)
                    {
                        if (pva.TargetPlayerId != PlayerControl.LocalPlayer.PlayerId && !CustomGameOptions.ImpostorSeeRoles)
                        {
                            pva.NameText.text = "<color=#A86629FF>" + pva.NameText.text + "</color>";
                        }
                    }
                    else pva.NameText.text = "<color=#FF0000>" + pva.NameText.text + "</color>";
                }
            }
        }

        public static float KillTimer()
        {
            var role = Role.GetRole(PlayerControl.LocalPlayer);
            if (role == null) return 0f;

            if (!PlayerControl.LocalPlayer.coolingDown()) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                role.KillCooldown -= Time.deltaTime;
                return role.KillCooldown;
            }
            else return role.KillCooldown;
        }
    }

    [HarmonyPatch(typeof(NormalGameManager), nameof(NormalGameManager.GetMapOptions))]
    public class MapPatch
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(ref MapOptions __result)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Data.IsImpostor()) return;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Spirit) && !Role.GetRole<Spirit>(PlayerControl.LocalPlayer).Caught) return;
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            if (CustomGameOptions.GameMode == GameMode.Chaos || CustomGameOptions.GameMode == GameMode.Werewolf) return;
            var aliveimp = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Impostors) && !x.Data.IsDead).ToList();
            if (aliveimp.Count > 1 && PlayerControl.LocalPlayer.Is(RoleEnum.Mafioso) && !PlayerControl.LocalPlayer.Data.IsDead) return;
            if (MeetingHud.Instance) return;

            __result = new MapOptions
		    {
			    Mode = MapOptions.Modes.Sabotage
		    };
        }
    }
}