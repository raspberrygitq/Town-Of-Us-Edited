using HarmonyLib;
using TownOfUsEdited.Roles.Modifiers;
using TownOfUsEdited.Roles;
using System.Linq;
using TownOfUsEdited.ImpostorRoles.TraitorMod;
using System.Collections;
using UnityEngine;
using TownOfUsEdited.Extensions;
using AmongUs.GameOptions;
using Reactor.Utilities;
using TownOfUsEdited.Patches.ImpostorRoles;
using TownOfUsEdited.Patches.CovenRoles;
using TownOfUsEdited.Patches.NeutralRoles.VampireMod;
using TownOfUsEdited.Patches.NeutralRoles.SerialKillerMod;
using TownOfUsEdited.Patches.Modifiers.LoversMod;
using TownOfUsEdited.Patches.CrewmateRoles.JailorMod;
using Reactor.Utilities.Extensions;
using TownOfUsEdited.CrewmateRoles.TimeLordMod;
using TownOfUsEdited.CrewmateRoles.MedicMod;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
    public class PlayerControl_Die
    {
        public static void Postfix()
        {
            // In case I wanna add code here
            CheckEnd();
        }

        public static void CheckEnd()
        {
            ImpostorChat.UpdateImpostorChat();
            CovenChat.UpdateCovenChat();
            VampireChat.UpdateVampireChat();
            SerialKillerChat.UpdateSerialKillerChat();
            LoversChat.UpdateLoversChat();
            JailorChat.UpdateJailorChat();
            if (!AmongUsClient.Instance.AmHost) return;
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return;
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.NetworkMode != NetworkModes.FreePlay) return;
            Coroutines.Start(CheckEndCriteria()); // Using a Coroutine to wait and make sure the dead people list gets updated
        }

        public static IEnumerator CheckEndCriteria()
        {
            yield return new WaitForSeconds(0.5f);

            var alives = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();

            if (AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay)
            {
                if (alives.Count <= 1)
                {
                    while (SpawnInMinigame.Instance || ExileController.Instance) yield return null;
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        bool isGhostRole = player.Is(RoleEnum.Guardian) || player.Is(RoleEnum.Spirit) || player.Is(RoleEnum.Haunter)
                        || player.Is(RoleEnum.Helper) || player.Is(RoleEnum.Blinder) || player.Is(RoleEnum.Freezer) || player.Is(RoleEnum.Phantom);
                        if (player.Data.IsDead && !isGhostRole) RevivePlayer(player);
                    }
                    HudManager.Instance.ShowPopUp("Normally, there wouldn't be enough players left alive so the game would've ended, but in Freeplay, we revive everyone instead.");
                }
                yield break;
            }

            foreach (var role in Role.AllRoles)
            {
                var impsAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x.Data.IsImpostor()).ToList();
                var traitorIsEnd = false;
                var modifiers = Modifier.GetModifiers(role.Player);
                if (modifiers != null && modifiers.Length != 0)
                {
                    foreach (var modifier in modifiers)
                    {
                        modifier.ModifierWin(GameManager.Instance.LogicFlow.Cast<LogicGameFlowNormal>());
                    }
                }
                if (SetTraitor.WillBeTraitor != null)
                {
                    if (SetTraitor.WillBeTraitor.Data.IsDead || SetTraitor.WillBeTraitor.Data.Disconnected || alives.Count < CustomGameOptions.LatestSpawn || impsAlive.Count * 2 >= alives.Count) traitorIsEnd = false;
                    else traitorIsEnd = true;
                }
                if (role.PauseEndCrit || traitorIsEnd || Role.ForceGameEnd || Role.NeutralEvilWin()) yield break;

                var aliveimps = PlayerControl.AllPlayerControls.ToArray().Where(x => (x.Is(Faction.Impostors) || x.Is(Faction.Madmates)) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var alivenk = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.NeutralKilling) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var alivecoven = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Coven) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var alivekillers = PlayerControl.AllPlayerControls.ToArray().Where(x => (x.Is(Faction.NeutralKilling) || x.Is(Faction.Impostors) || x.Is(Faction.Coven)) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var alivesnonkiller = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Impostors) && !x.Is(Faction.NeutralKilling) && !x.Is(Faction.Madmates) && !x.Is(Faction.Coven) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var alivecrewkiller = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && x.IsCrewKiller() && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var alivevamps = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Vampire) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var alivesks = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.SerialKiller) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var alivelovers = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(ModifierEnum.Lover) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();

                if (alivelovers != null && alivelovers.Count < 2) role.NeutralWin(GameManager.Instance.LogicFlow.Cast<LogicGameFlowNormal>());
                else if (alivelovers == null) role.NeutralWin(GameManager.Instance.LogicFlow.Cast<LogicGameFlowNormal>());
                if (alivenk.Count <= 0 && alivecoven.Count <= 0 && alivesnonkiller.Count <= aliveimps.Count && CustomGameOptions.GameMode != GameMode.Chaos && aliveimps.Count > 0 && (alivecrewkiller.Count <= 0 || !CustomGameOptions.CrewKillersContinue) && alivelovers.Count < 2)
                {
                    Role.ImpostorWins = true;
                    Utils.Rpc(CustomRPC.ImpostorWin);
                    Utils.EndGame(GameOverReason.ImpostorsByVote);
                    PluginSingleton<TownOfUsEdited>.Instance.Log.LogMessage("GAME OVER REASON: Impostor Win");
                    yield break;
                }
                else if (alivenk.Count <= 0 && aliveimps.Count <= 0 && alivesnonkiller.Count <= alivecoven.Count && alivecoven.Count > 0 && (alivecrewkiller.Count <= 0 || !CustomGameOptions.CrewKillersContinue) && alivelovers.Count < 2)
                {
                    Role.CovenWins = true;
                    Utils.Rpc(CustomRPC.CovenWin);
                    Utils.EndGame();
                    PluginSingleton<TownOfUsEdited>.Instance.Log.LogMessage("GAME OVER REASON: Coven Win");
                    yield break;
                }
                else if (alivekillers.Count <= 0 && CustomGameOptions.GameMode != GameMode.BattleRoyale)
                {
                    Role.CrewmateWins = true;
                    Utils.Rpc(CustomRPC.CrewmateWin);
                    Utils.EndGame(GameOverReason.CrewmatesByVote);
                    PluginSingleton<TownOfUsEdited>.Instance.Log.LogMessage("GAME OVER REASON: No Killers Left");
                    yield break;
                }
            }

            yield break;
        }

        public static void RevivePlayer(PlayerControl player)
        {
            var position = player.GetTruePosition();
            var parentId = player.PlayerId;

            foreach (var poisoner in Role.GetRoles(RoleEnum.Poisoner))
            {
                var poisonerRole = (Poisoner)poisoner;
                if (poisonerRole.PoisonedPlayer == player) poisonerRole.PoisonedPlayer = null;
            }

            if (PlayerControl.LocalPlayer == player)
            {
                var position2 = PlayerControl.LocalPlayer.transform.position;
                TimeLordPatches.Positions.Add((Vector2.zero, Time.time, "Teleport", position2, 0, null));
            }

            player.Revive();
            RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);
            Murder.KilledPlayers.Remove(
                Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == player.PlayerId));
            var usedPosition = new Vector2(position.x, position.y + 0.3636f);
            player.transform.position = new Vector2(usedPosition.x, usedPosition.y);

            if (Patches.SubmergedCompatibility.isSubmerged() && PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
            {
                Patches.SubmergedCompatibility.ChangeFloor(player.transform.position.y > -7);
            }
            foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
            {
                if (deadBody.ParentId == player.PlayerId) Object.Destroy(deadBody.gameObject);
            }

            if (player.IsLover() && CustomGameOptions.BothLoversDie)
            {
                var lover = Modifier.GetModifier<Lover>(player).OtherLover.Player;

                foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
                {
                    if (deadBody.ParentId == lover.PlayerId)
                    {
                        lover.Revive();
                        RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);
                        Murder.KilledPlayers.Remove(
                            Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == lover.PlayerId));
                        
                        if (PlayerControl.LocalPlayer == lover)
                        {
                            lover.myTasks.RemoveAt(1);
                            var position3 = PlayerControl.LocalPlayer.transform.position;
                            TimeLordPatches.Positions.Add((Vector2.zero, Time.time, "Teleport", position3, 0, null));
                        }

                        var position2 = deadBody.TruePosition;
                        var usedPosition2 = new Vector2(position2.x, position2.y + 0.3636f);
                        lover.transform.position = new Vector2(usedPosition2.x, usedPosition2.y);

                        if (Patches.SubmergedCompatibility.isSubmerged() && PlayerControl.LocalPlayer.PlayerId == lover.PlayerId)
                        {
                            Patches.SubmergedCompatibility.ChangeFloor(lover.transform.position.y > -7);
                        }
                        deadBody.gameObject.Destroy();
                    }
                }
            }
            try
            {
                Minigame.Instance.Close();
                Minigame.Instance.Close();
            }
            catch
            { }

            if (PlayerControl.LocalPlayer == player)
            {
                Utils.ShowDeadBodies = false;
                player.myTasks.RemoveAt(1);
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Exiled))]
    public class PlayerControl_Exiled
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (__instance == PlayerControl.LocalPlayer) Utils.UpdateTaskText(PlayerControl.LocalPlayer);
        }
    }
}