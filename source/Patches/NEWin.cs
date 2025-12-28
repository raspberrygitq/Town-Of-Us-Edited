using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Roles;
using TownOfUsEdited.Roles.Modifiers;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class NEWin
    {
        public static void Postfix(EndGameManager __instance)
        {
            if (CustomGameOptions.NeutralEvilWinEndsGame) return;
            var neWin = false;
            var doomRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Doomsayer && ((Doomsayer)x).WonByGuessing && ((Doomsayer)x).Player == PlayerControl.LocalPlayer);
            if (doomRole != null) neWin = true;
            var vultureRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Vulture && ((Vulture)x).VultureWins && ((Vulture)x).Player == PlayerControl.LocalPlayer);
            if (vultureRole != null) neWin = true;
            var exeRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Executioner && ((Executioner)x).TargetVotedOut && ((Executioner)x).Player == PlayerControl.LocalPlayer);
            if (exeRole != null) neWin = true;
            var jestRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Jester && ((Jester)x).VotedOut && ((Jester)x).Player == PlayerControl.LocalPlayer);
            if (jestRole != null) neWin = true;
            var trollRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Troll && ((Troll)x).TrolledVotedOut && ((Troll)x).Player == PlayerControl.LocalPlayer);
            if (trollRole != null) neWin = true;
            var phantomRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Phantom && ((Phantom)x).CompletedTasks && ((Phantom)x).Player == PlayerControl.LocalPlayer);
            if (phantomRole != null) neWin = true;
            if (neWin)
            {
                __instance.WinText.text = "</color><color=#008DFFFF>Victory";
                var loveRole = Modifier.AllModifiers.FirstOrDefault(x => x.ModifierType == ModifierEnum.Lover && ((Lover)x).LoveCoupleWins);
                if (loveRole != null) return;
                var survRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Survivor && Role.SurvOnlyWins);
                if (survRole != null) return;
                var vampRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Vampire && Role.VampireWins);
                if (vampRole != null) return;
                var skRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.SerialKiller && Role.SKWins);
                if (skRole != null) return;
                var arsoRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Arsonist && ((Arsonist)x).ArsonistWins);
                if (arsoRole != null) return;
                var doppelRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Doppelganger && ((Doppelganger)x).DoppelgangerWins);
                if (doppelRole != null) return;
                var glitchRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Glitch && ((Glitch)x).GlitchWins);
                if (glitchRole != null) return;
                var mutantRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Mutant && ((Mutant)x).MutantWins);
                if (mutantRole != null) return;
                var infectiousRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Infectious && ((Infectious)x).InfectiousWins);
                if (infectiousRole != null) return;
                var juggRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Juggernaut && ((Juggernaut)x).JuggernautWins);
                if (juggRole != null) return;
                var pestRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Pestilence && ((Pestilence)x).PestilenceWins);
                if (pestRole != null) return;
                var pbRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Plaguebearer && ((Plaguebearer)x).PlaguebearerWins);
                if (pbRole != null) return;
                var attackRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Attacker && ((Attacker)x).AttackerWins);
                if (attackRole != null) return;
                var terroristRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Terrorist && ((Terrorist)x).TerroristWins);
                if (terroristRole != null) return;
                var wwRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Werewolf && ((Werewolf)x).WerewolfWins);
                if (wwRole != null) return;
                var scRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.SoulCollector && ((SoulCollector)x).SCWins);
                if (scRole != null) return;
                __instance.BackgroundBar.material.SetColor("_Color", Role.GetRole(PlayerControl.LocalPlayer).Color);
            }
        }
    }
}