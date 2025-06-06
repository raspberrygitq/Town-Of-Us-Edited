using System.Linq;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using TownOfUsEdited.Roles;
using TownOfUsEdited.Roles.Modifiers;

namespace TownOfUsEdited
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public class AmongUsClientGameEnd
    {
        public static void Postfix()
        {
            if (Role.NobodyWins)
            {
                EndGameResult.CachedWinners = new List<CachedPlayerData>();
                return;
            }
            if (Role.ForceGameEnd)
            {
                EndGameResult.CachedWinners = new List<CachedPlayerData>();
                return;
            }
            if (Role.SurvOnlyWins)
            {
                EndGameResult.CachedWinners = new List<CachedPlayerData>();
                foreach (var role in Role.GetRoles(RoleEnum.Survivor))
                {
                    var surv = (Survivor)role;
                    if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                    {
                        var survData = new CachedPlayerData(surv.Player.Data);
                        if (PlayerControl.LocalPlayer != surv.Player) survData.IsYou = false;
                        EndGameResult.CachedWinners.Add(survData);
                    }
                }

                return;
            }

            if (CustomGameOptions.NeutralEvilWinEndsGame)
            {
                foreach (var role in Role.AllRoles)
                {
                    var type = role.RoleType;

                    if (type == RoleEnum.Jester)
                    {
                        var jester = (Jester)role;
                        if (jester.VotedOut)
                        {
                            EndGameResult.CachedWinners = new List<CachedPlayerData>();
                            var jestData = new CachedPlayerData(jester.Player.Data);
                            jestData.IsDead = false;
                            if (PlayerControl.LocalPlayer != jester.Player) jestData.IsYou = false;
                            EndGameResult.CachedWinners.Add(jestData);
                            if (jester.Player.IsLover() && CustomGameOptions.NeutralEvilWinsLover)
                            {
                                var loverModifier = Modifier.GetModifier<Lover>(jester.Player);
                                var otherLover = loverModifier.OtherLover;
                                var otherLoverData = new CachedPlayerData(otherLover.Player.Data);
                                if (PlayerControl.LocalPlayer != otherLover.Player) otherLoverData.IsYou = false;
                                EndGameResult.CachedWinners.Add(otherLoverData);
                            }
                            return;
                        }
                    }
                    else if (type == RoleEnum.Executioner)
                    {
                        var executioner = (Executioner)role;
                        if (executioner.TargetVotedOut)
                        {
                            EndGameResult.CachedWinners = new List<CachedPlayerData>();
                            var exeData = new CachedPlayerData(executioner.Player.Data);
                            if (PlayerControl.LocalPlayer != executioner.Player) exeData.IsYou = false;
                            EndGameResult.CachedWinners.Add(exeData);
                            if (executioner.Player.IsLover() && CustomGameOptions.NeutralEvilWinsLover)
                            {
                                var loverModifier = Modifier.GetModifier<Lover>(executioner.Player);
                                var otherLover = loverModifier.OtherLover;
                                var otherLoverData = new CachedPlayerData(otherLover.Player.Data);
                                if (PlayerControl.LocalPlayer != otherLover.Player) otherLoverData.IsYou = false;
                                EndGameResult.CachedWinners.Add(otherLoverData);
                            }
                            return;
                        }
                    }
                    else if (type == RoleEnum.Doomsayer)
                    {
                        var doom = (Doomsayer)role;
                        if (doom.WonByGuessing)
                        {
                            EndGameResult.CachedWinners = new List<CachedPlayerData>();
                            var doomData = new CachedPlayerData(doom.Player.Data);
                            if (PlayerControl.LocalPlayer != doom.Player) doomData.IsYou = false;
                            EndGameResult.CachedWinners.Add(doomData);
                            if (doom.Player.IsLover() && CustomGameOptions.NeutralEvilWinsLover)
                            {
                                var loverModifier = Modifier.GetModifier<Lover>(doom.Player);
                                var otherLover = loverModifier.OtherLover;
                                var otherLoverData = new CachedPlayerData(otherLover.Player.Data);
                                if (PlayerControl.LocalPlayer != otherLover.Player) otherLoverData.IsYou = false;
                                EndGameResult.CachedWinners.Add(otherLoverData);
                            }
                            return;
                        }
                    }
                    else if (type == RoleEnum.Vulture)
                    {
                        var vulture = (Vulture)role;
                        if (vulture.VultureWins)
                        {
                            EndGameResult.CachedWinners = new List<CachedPlayerData>();
                            var vultureData = new CachedPlayerData(vulture.Player.Data);
                            if (PlayerControl.LocalPlayer != vulture.Player) vultureData.IsYou = false;
                            EndGameResult.CachedWinners.Add(vultureData);
                            if (vulture.Player.IsLover() && CustomGameOptions.NeutralEvilWinsLover)
                            {
                                var loverModifier = Modifier.GetModifier<Lover>(vulture.Player);
                                var otherLover = loverModifier.OtherLover;
                                var otherLoverData = new CachedPlayerData(otherLover.Player.Data);
                                if (PlayerControl.LocalPlayer != otherLover.Player) otherLoverData.IsYou = false;
                                EndGameResult.CachedWinners.Add(otherLoverData);
                            }
                            return;
                        }
                    }
                    else if (type == RoleEnum.Troll)
                    {
                        var troll = (Troll)role;
                        if (troll.TrolledVotedOut)
                        {
                            EndGameResult.CachedWinners = new List<CachedPlayerData>();
                            var trollData = new CachedPlayerData(troll.Player.Data);
                            if (PlayerControl.LocalPlayer != troll.Player) trollData.IsYou = false;
                            EndGameResult.CachedWinners.Add(trollData);
                            if (troll.Player.IsLover() && CustomGameOptions.NeutralEvilWinsLover)
                            {
                                var loverModifier = Modifier.GetModifier<Lover>(troll.Player);
                                var otherLover = loverModifier.OtherLover;
                                var otherLoverData = new CachedPlayerData(otherLover.Player.Data);
                                if (PlayerControl.LocalPlayer != otherLover.Player) otherLoverData.IsYou = false;
                                EndGameResult.CachedWinners.Add(otherLoverData);
                            }
                            return;
                        }
                    }
                    else if (type == RoleEnum.Phantom)
                    {
                        var phantom = (Phantom)role;
                        if (phantom.CompletedTasks)
                        {
                            EndGameResult.CachedWinners = new List<CachedPlayerData>();
                            var phantomData = new CachedPlayerData(phantom.Player.Data);
                            if (PlayerControl.LocalPlayer != phantom.Player) phantomData.IsYou = false;
                            EndGameResult.CachedWinners.Add(phantomData);
                            return;
                        }
                    }
                }
            }

            foreach (var modifier in Modifier.AllModifiers)
            {
                var type = modifier.ModifierType;

                if (type == ModifierEnum.Lover)
                {
                    var lover = (Lover)modifier;
                    if (lover.LoveCoupleWins)
                    {
                        var otherLover = lover.OtherLover;
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var loverOneData = new CachedPlayerData(lover.Player.Data);
                        var loverTwoData = new CachedPlayerData(otherLover.Player.Data);
                        if (PlayerControl.LocalPlayer != lover.Player) loverOneData.IsYou = false;
                        if (PlayerControl.LocalPlayer != otherLover.Player) loverTwoData.IsYou = false;
                        EndGameResult.CachedWinners.Add(loverOneData);
                        EndGameResult.CachedWinners.Add(loverTwoData);
                        return;
                    }
                }
            }

            if (Role.VampireWins)
            {
                EndGameResult.CachedWinners = new List<CachedPlayerData>();
                foreach (var role in Role.GetRoles(RoleEnum.Vampire))
                {
                    var vamp = (Vampire)role;
                    var vampData = new CachedPlayerData(vamp.Player.Data);
                    if (PlayerControl.LocalPlayer != vamp.Player) vampData.IsYou = false;
                    EndGameResult.CachedWinners.Add(vampData);
                }
            }

            if (Role.ImpostorWins)
            {
                EndGameResult.CachedWinners = new List<CachedPlayerData>();
                foreach (var role in Role.GetFactions(Faction.Impostors))
                {
                    var impData = new CachedPlayerData(role.Player.Data);
                    impData.IsImpostor = true;
                    if (PlayerControl.LocalPlayer != role.Player) impData.IsYou = false;
                    EndGameResult.CachedWinners.Add(impData);
                }
                foreach (var role in Role.GetFactions(Faction.Madmates))
                {
                    var madData = new CachedPlayerData(role.Player.Data);
                    madData.IsImpostor = true;
                    if (PlayerControl.LocalPlayer != role.Player) madData.IsYou = false;
                    EndGameResult.CachedWinners.Add(madData);
                }
            }

            if (Role.CrewmateWins)
            {
                EndGameResult.CachedWinners = new List<CachedPlayerData>();
                foreach (var role in Role.GetFactions(Faction.Crewmates))
                {
                    var crewData = new CachedPlayerData(role.Player.Data);
                    if (PlayerControl.LocalPlayer != role.Player) crewData.IsYou = false;
                    if (!role.Player.Is(RoleEnum.Spectator))
                    {
                        EndGameResult.CachedWinners.Add(crewData);
                    }
                }
            }

            if (Role.SKWins)
            {
                EndGameResult.CachedWinners = new List<CachedPlayerData>();
                foreach (var role in Role.GetRoles(RoleEnum.SerialKiller))
                {
                    var sk = (SerialKiller)role;
                    var skData = new CachedPlayerData(sk.Player.Data);
                    if (PlayerControl.LocalPlayer != sk.Player) skData.IsYou = false;
                    EndGameResult.CachedWinners.Add(skData);
                }
            }

            if (Role.CovenWins)
            {
                EndGameResult.CachedWinners = new List<CachedPlayerData>();
                foreach (var role in Role.GetFactions(Faction.Coven))
                {
                    var covenData = new CachedPlayerData(role.Player.Data);
                    if (PlayerControl.LocalPlayer != role.Player) covenData.IsYou = false;
                    EndGameResult.CachedWinners.Add(covenData);
                }
            }

            foreach (var role in Role.AllRoles)
            {
                var type = role.RoleType;

                if (type == RoleEnum.Glitch)
                {
                    var glitch = (Glitch)role;
                    if (glitch.GlitchWins)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var glitchData = new CachedPlayerData(glitch.Player.Data);
                        if (PlayerControl.LocalPlayer != glitch.Player) glitchData.IsYou = false;
                        EndGameResult.CachedWinners.Add(glitchData);
                    }
                }
                else if (type == RoleEnum.SoulCollector)
                {
                    var sc = (SoulCollector)role;
                    if (sc.SCWins)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var scData = new CachedPlayerData(sc.Player.Data);
                        if (PlayerControl.LocalPlayer != sc.Player) scData.IsYou = false;
                        EndGameResult.CachedWinners.Add(scData);
                    }
                }
                else if (type == RoleEnum.Player)
                {
                    var player = (Player)role;
                    if (player.PlayerWins)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var playerData = new CachedPlayerData(player.Player.Data);
                        if (PlayerControl.LocalPlayer != player.Player) playerData.IsYou = false;
                        EndGameResult.CachedWinners.Add(playerData);
                    }
                }
                else if (type == RoleEnum.Mutant)
                {
                    var mutant = (Mutant)role;
                    if (mutant.MutantWins)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var mutantData = new CachedPlayerData(mutant.Player.Data);
                        if (PlayerControl.LocalPlayer != mutant.Player) mutantData.IsYou = false;
                        EndGameResult.CachedWinners.Add(mutantData);
                    }
                }
                else if (type == RoleEnum.Infectious)
                {
                    var infectious = (Infectious)role;
                    if (infectious.InfectiousWins)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var infectiousData = new CachedPlayerData(infectious.Player.Data);
                        if (PlayerControl.LocalPlayer != infectious.Player) infectiousData.IsYou = false;
                        EndGameResult.CachedWinners.Add(infectiousData);
                    }
                }
                else if (type == RoleEnum.Juggernaut)
                {
                    var juggernaut = (Juggernaut)role;
                    if (juggernaut.JuggernautWins)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var juggData = new CachedPlayerData(juggernaut.Player.Data);
                        if (PlayerControl.LocalPlayer != juggernaut.Player) juggData.IsYou = false;
                        EndGameResult.CachedWinners.Add(juggData);
                    }
                }
                else if (type == RoleEnum.Arsonist)
                {
                    var arsonist = (Arsonist)role;
                    if (arsonist.ArsonistWins)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var arsonistData = new CachedPlayerData(arsonist.Player.Data);
                        if (PlayerControl.LocalPlayer != arsonist.Player) arsonistData.IsYou = false;
                        EndGameResult.CachedWinners.Add(arsonistData);
                    }
                }
                else if (type == RoleEnum.Doppelganger)
                {
                    var doppel = (Doppelganger)role;
                    if (doppel.DoppelgangerWins)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var doppelData = new CachedPlayerData(doppel.Player.Data);
                        if (PlayerControl.LocalPlayer != doppel.Player) doppelData.IsYou = false;
                        EndGameResult.CachedWinners.Add(doppelData);
                    }
                }
                else if (type == RoleEnum.Plaguebearer)
                {
                    var plaguebearer = (Plaguebearer)role;
                    if (plaguebearer.PlaguebearerWins)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var pbData = new CachedPlayerData(plaguebearer.Player.Data);
                        if (PlayerControl.LocalPlayer != plaguebearer.Player) pbData.IsYou = false;
                        EndGameResult.CachedWinners.Add(pbData);
                    }
                }
                else if (type == RoleEnum.Attacker)
                {
                    var attacker = (Attacker)role;
                    if (attacker.AttackerWins)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var attackData = new CachedPlayerData(attacker.Player.Data);
                        if (PlayerControl.LocalPlayer != attacker.Player) attackData.IsYou = false;
                        EndGameResult.CachedWinners.Add(attackData);
                    }
                }
                else if (type == RoleEnum.Terrorist)
                {
                    var terrorist = (Terrorist)role;
                    if (terrorist.TerroristWins)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var terroristData = new CachedPlayerData(terrorist.Player.Data);
                        if (PlayerControl.LocalPlayer != terrorist.Player) terroristData.IsYou = false;
                        EndGameResult.CachedWinners.Add(terroristData);
                    }
                }
                else if (type == RoleEnum.Pestilence)
                {
                    var pestilence = (Pestilence)role;
                    if (pestilence.PestilenceWins)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var pestilenceData = new CachedPlayerData(pestilence.Player.Data);
                        if (PlayerControl.LocalPlayer != pestilence.Player) pestilenceData.IsYou = false;
                        EndGameResult.CachedWinners.Add(pestilenceData);
                    }
                }
                else if (type == RoleEnum.Maul)
                {
                    var werewolf = (Maul)role;
                    if (werewolf.WerewolfWins)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var werewolfData = new CachedPlayerData(werewolf.Player.Data);
                        if (PlayerControl.LocalPlayer != werewolf.Player) werewolfData.IsYou = false;
                        EndGameResult.CachedWinners.Add(werewolfData);
                    }
                }
                else if (type == RoleEnum.WhiteWolf)
                {
                    var whitewolf = (WhiteWolf)role;
                    if (whitewolf.WhiteWolfWins)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var whitewolfData = new CachedPlayerData(whitewolf.Player.Data);
                        if (PlayerControl.LocalPlayer != whitewolf.Player) whitewolfData.IsYou = false;
                        EndGameResult.CachedWinners.Add(whitewolfData);
                    }
                }
            }

            foreach (var role in Role.GetRoles(RoleEnum.Survivor))
            {
                var surv = (Survivor)role;
                if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                {
                    var isImp = EndGameResult.CachedWinners.Count != 0 && EndGameResult.CachedWinners[0].IsImpostor;
                    var survWinData = new CachedPlayerData(surv.Player.Data);
                    if (isImp) survWinData.IsImpostor = true;
                    if (PlayerControl.LocalPlayer != surv.Player) survWinData.IsYou = false;
                    EndGameResult.CachedWinners.Add(survWinData);
                }
            }
            foreach (var role in Role.GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel)role;
                var gaTargetData = new CachedPlayerData(ga.target.Data);
                foreach (CachedPlayerData winner in EndGameResult.CachedWinners.ToArray())
                {
                    if (gaTargetData.ColorId == winner.ColorId)
                    {
                        var isImp = EndGameResult.CachedWinners[0].IsImpostor;
                        var gaWinData = new CachedPlayerData(ga.Player.Data);
                        if (isImp) gaWinData.IsImpostor = true;
                        if (PlayerControl.LocalPlayer != ga.Player) gaWinData.IsYou = false;
                        EndGameResult.CachedWinners.Add(gaWinData);
                    }
                }
            }
            foreach (var role in Role.GetRoles(RoleEnum.Mercenary))
            {
                var merc = (Mercenary)role;
                foreach (var bribeId in merc.Bribed)
                {
                    var bribe = Utils.PlayerById(bribeId);
                    if (bribe == null || bribe.Data == null || bribe.Data.IsDead || bribe.Data.Disconnected || bribe.Is(RoleEnum.Mercenary)) continue;
                    var bribedData = new CachedPlayerData(bribe.Data);
                    if (EndGameResult.CachedWinners.ToArray().Where(x => x.PlayerName == bribedData.PlayerName).ToList().Count > 0)
                    {
                        var isImp = EndGameResult.CachedWinners[0].IsImpostor;
                        var mercWinData = new CachedPlayerData(merc.Player.Data);
                        if (isImp) mercWinData.IsImpostor = true;
                        if (PlayerControl.LocalPlayer != merc.Player) mercWinData.IsYou = false;
                        EndGameResult.CachedWinners.Add(mercWinData);
                    }
                }
            }
        }
    }
}
