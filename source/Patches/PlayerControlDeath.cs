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

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
    public class PlayerControl_Die
    {
        public static void Postfix()
        {
            if (!AmongUsClient.Instance.AmHost) return;
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return;
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            Coroutines.Start(CheckEndCriteria()); // Using a Coroutine to wait and make sure the dead people list gets updated
        }

        public static IEnumerator CheckEndCriteria()
        {
            yield return new WaitForSeconds(0.5f);

            foreach (var role in Role.AllRoles)
            {
                var alives = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var impsAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x.Data.IsImpostor()).ToList();
                var traitorIsEnd = false;
                var modifier = Modifier.GetModifier(role.Player);
                if (modifier != null) modifier.ModifierWin(GameManager.Instance.LogicFlow.Cast<LogicGameFlowNormal>());
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Is(RoleEnum.Astral) && Role.GetRole<Astral>(player).Enabled)
                    {
                        alives.Add(player);
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

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Is(RoleEnum.Astral) && Role.GetRole<Astral>(player).Enabled)
                    {
                        alivesnonkiller.Add(player);
                        if (player.IsLover()) alivelovers.Add(player);
                    }
                }

                if (alivelovers.Count < 2) role.NeutralWin(GameManager.Instance.LogicFlow.Cast<LogicGameFlowNormal>());
                if (alivenk.Count <= 0 && alivecoven.Count <= 0 && alivesnonkiller.Count <= aliveimps.Count && CustomGameOptions.GameMode != GameMode.Chaos && aliveimps.Count > 0 && (alivecrewkiller.Count <= 0 || !CustomGameOptions.CrewKillersContinue) && alivelovers.Count < 2)
                {
                    Role.ImpostorWins = true;
                    Utils.Rpc(CustomRPC.ImpostorWin);
                    Utils.EndGame(GameOverReason.ImpostorByVote);
                    System.Console.WriteLine("GAME OVER REASON: Impostor Victory");
                    yield break;
                }
                else if (alivenk.Count <= 0 && aliveimps.Count <= 0 && alivesnonkiller.Count <= alivecoven.Count && alivecoven.Count > 0 && (alivecrewkiller.Count <= 0 || !CustomGameOptions.CrewKillersContinue) && alivelovers.Count < 2)
                {
                    Role.CovenWins = true;
                    Utils.Rpc(CustomRPC.CovenWin);
                    Utils.EndGame();
                    System.Console.WriteLine("GAME OVER REASON: Coven Win");
                    yield break;
                }
                else if (alivekillers.Count <= 0 && CustomGameOptions.GameMode != GameMode.BattleRoyale)
                {
                    Role.CrewmateWins = true;
                    Utils.Rpc(CustomRPC.CrewmateWin);
                    Utils.EndGame(GameOverReason.HumansByVote);
                    System.Console.WriteLine("GAME OVER REASON: No Killers Left");
                    yield break;
                }
            }

            yield break;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Exiled))]
    public class PlayerControl_Exiled
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (__instance == PlayerControl.LocalPlayer) Coroutines.Start(Utils.UpdateTaskText(PlayerControl.LocalPlayer));
        }
    }
}