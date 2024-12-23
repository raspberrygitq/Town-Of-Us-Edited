using HarmonyLib;
using UnityEngine;

namespace TownOfUs.Roles
{
    [HarmonyPatch(typeof(HudManager))]
    public class UpdateRoleInfo
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                var pc = PlayerControl.LocalPlayer;
                // Spectator
                if (pc.Is(RoleEnum.Spectator))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#da6f00>Spectator</color>, enjoy watching the game.");
                }
                // Crewmates Roles infos
                if (pc.Is(RoleEnum.Crewmate))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#00FFFF>Crewmate</color>, do you tasks and find out who the <color=#FF0000>Impostors</color> are!");
                }
                if (pc.Is(RoleEnum.Impostor))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Impostor</color>, kill everyone and sabotage without being caught!");
                }
                if (pc.Is(RoleEnum.Aurial))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#B34D99FF>Aurial</color>, you can see auras of different players by radiating.\nHowever, you can not see who is who.");
                }
                if (pc.Is(RoleEnum.Captain))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#6B8DE1>Captain</color>, use your zoom ability to increase your vision.");
                }
                if (pc.Is(RoleEnum.Superstar))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#ffca00>Superstar</color>, when you die, all players will get notified with a flash on their screen.");
                }
                if (pc.Is(RoleEnum.Avenger))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#216f01>Avenger</color>, you can avenge a dead body if you find one, resulting in being able to kill his killer.");
                }
                if (pc.Is(RoleEnum.Chameleon) && CustomGameOptions.GameMode != GameMode.Werewolf)
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#068c38>Chameleon</color>, use your ability to turn invisible and watch other players.");
                }
                if (pc.Is(RoleEnum.Detective))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#4D4DFFFF>Detective</color>, inspect dead bodies to find out their killer.");
                }
                if (pc.Is(RoleEnum.Haunter))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#D3D3D3FF>Haunter</color>, finish your tasks to reveal the <color=#FF0000FF>Impostors</color> to everyone.");
                }
                if (pc.Is(RoleEnum.Helper))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#7cb6c2>Helper</color>, you can use your ability on a player to alert them from a nearby danger.\nThe alerted player will get a pop up saying that someone alerted them and will get a speed boost during the Alert duration.");
                }
                if (pc.Is(RoleEnum.Guardian))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#67bb43>Guardian</color>, you can use your ability on a player to make them immune to direct kills during a certain amount of time.");
                }
                if (pc.Is(RoleEnum.Investigator))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#00B3B3FF>Investigator</color>, inspect footprints to know where anyone came from.");
                }
                if (pc.Is(RoleEnum.Mystic))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#4D99E6FF>Mystic</color>, when someone gets killed, you will see a pop up in real time indicating who is dead.");
                }
                if (pc.Is(RoleEnum.Oracle))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#BF00BFFF>Oracle</color>, you can gain informations by making a player confess to you.\nAdditionally, the confessed player can not be ejected and his alignement will be revealed to everyone if you die.");
                }
                if (pc.Is(RoleEnum.Seer) && CustomGameOptions.GameMode != GameMode.Werewolf)
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FFCC80FF>Seer</color>, you can check if someone is evil or not.\nUse this ability on suspicious players!");
                }
                if (pc.Is(RoleEnum.Snitch))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#D4AF37FF>Snitch</color>, you can see all <color=#FF0000FF>Impostors</color> after completing all your tasks.\nBe careful because <color=#FF0000FF>Impostors</color> will also know who you are after completing a certain amount of tasks.");
                }
                if (pc.Is(RoleEnum.Spy))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#CCA3CCFF>Spy</color>, you can see anyone's location on the admin map.");
                }
                if (pc.Is(RoleEnum.Tracker))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#009900FF>Tracker</color>, you can track anyone's location in real time.");
                }
                if (pc.Is(RoleEnum.Trapper))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#A7D1B3FF>Trapper</color>, place traps to gain informations about other player's roles.");
                }
                if (pc.Is(RoleEnum.Fighter))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#9358e6>Fighter</color>, you can kill anyone but if you kill a Crewmate, you will turn into a Madmate.");
                }
                if (pc.Is(RoleEnum.Hunter))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#29AB87FF>Hunter</color>, you can execute any suspicious player if they have interacted with someone else.");
                }
                if (pc.Is(RoleEnum.Jailor))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#61a37a>Jailor</color>, you can jail anyone during the round making them unable to kill or interact with anyone else, then on the next meeting, you will be able to execute them.\nIf you execute a Crewmate, you lose your ability and might even die too.");
                }
                if (pc.Is(RoleEnum.Deputy))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#ed9a47>Deputy</color>, during meetings, you can shoot someone, but only once.\nChoose carefully.");
                }
                if (pc.Is(RoleEnum.Knight))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#ffd082>Knight</color>, you can kill anyone but only once.\nUse this ability with responsibility.");
                }
                if (pc.Is(RoleEnum.Sheriff) && CustomGameOptions.GameMode != GameMode.Werewolf)
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FFFF00FF>Sheriff</color>, you can kill the <color=#FF0000FF>Impostors</color> but if you try to kill a Crewmate, you will die instead.");
                }
                if (pc.Is(RoleEnum.VampireHunter))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#B3B3E6FF>Vampire Hunter</color>, you can cure converted <color=#262626FF>Vampires</color> from the infections making them recover their old role.\nIf you try to cure the original <color=#262626FF>Vampire</color>, you will kill them!\nIf all <color=#262626FF>Vampires</color> are dead, you will turn into another Crewmate role.");
                }
                if (pc.Is(RoleEnum.Veteran))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#998040FF>Veteran</color>, you can alert to kill anyone that interacts with you (including non killers).\nUse this ability to protect yourself against attacks.");
                }
                if (pc.Is(RoleEnum.Astral))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#a703ae>Astral</color>, you can use your ability to leave your body behind you and project your spirit.\nHowever, if anyone cleans / eat your body while in ghost form, you die.");
                }
                if (pc.Is(RoleEnum.Vigilante))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FFFF99FF>Vigilante</color>, you can guess <color=#FF0000FF>Impostor</color>'s roles in meeting.\nHowever, if you guess a role incorreclty, you will die instead.");
                }
                if (pc.Is(RoleEnum.Altruist))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#660000FF>Altruist</color>, you can sacrifice yourself to revive someone else.\nHowever, any evil player will be alerted that you revived someone.");
                }
                if (pc.Is(RoleEnum.Medic))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#006600FF>Medic</color>, you can give a shield to someone which will make him invincible to murder attempts.\nIf the shield break, you may be noticed.\nYou also gain informations by reporting dead bodies.");
                }
                if (pc.Is(RoleEnum.Doctor))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#00EE3C>Doctor</color>, use your abilities to resurect other players!\nYou can resurect anyone so be careful while using your powers.\nNote that revived players can't chat.");
                }
                if (pc.Is(RoleEnum.Crusader))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#815230>Crusader</color>, you can Crusade someone resulting in killing anyone that interacts with that player.\nYou can only Crusade one player at the same time.\nCrusade will be reset every meeting.");
                }
                if (pc.Is(RoleEnum.Bodyguard))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#255bc9>Bodyguard</color>, if a near player tries to kill someone else, you will kill them and die too.\nNote that this only applies to players that use the regular kill method.");
                }
                if (pc.Is(RoleEnum.Paranoïac) && CustomGameOptions.GameMode != GameMode.Werewolf)
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#0a7eae>Paranoïac</color>, you can call a Meeting at anytime and hide in vents if you feel scared.");
                }
                if (pc.Is(RoleEnum.Engineer))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FFA60AFF>Engineer</color>, you can use vents to catch <color=#FF0000FF>Impostors</color> and you can also fix sabotages.");
                }
                if (pc.Is(RoleEnum.Lighter))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FFA60AFF>Lighter</color>, you can use ability to get an Impostor vision for a short period of time.\nAlso works when lights are off.");
                }
                if (pc.Is(RoleEnum.TimeLord))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#0f00b8>Time Lord</color>, you can reverse the time making players currently unable to move and reviving players that died recently.");
                }
                if (pc.Is(RoleEnum.Imitator))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#B3D94DFF>Imitator</color>, you can imitate the role of dead Crewmates.\nNote that not all roles are imitables.");
                }
                if (pc.Is(RoleEnum.Warden))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#9900FFFF>Warden</color>, you can use your button on someone to fortify them, they will be immune to interactions and if someone does interact with them, you and your target will get notified.\nNote that this doesn't protect from kills.");
                }
                if (pc.Is(RoleEnum.Mayor) && CustomGameOptions.GameMode != GameMode.Werewolf)
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#704FA8FF>Mayor</color>, you can choose to reveal to other players which will make you unguessable and will safe you.\nYou will also gain an extra vote but your vision will be lower.");
                }
                if (pc.Is(RoleEnum.Medium))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#A680FFFF>Medium</color>, you can see the ghosts of recent dead players by pressing the Mediate button.\nGhosts might lead you to their killer but be careful because some might try to trick you!");
                }
                if (pc.Is(RoleEnum.Prosecutor) && CustomGameOptions.GameMode != GameMode.Werewolf)
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#B38000FF>Prosecutor</color>, you can choose to control the vote and vote someone out once.\nHowever, you may also die by executing a Crewmate.\nYou can not prosecute on the 10 last seconds of a meeting.");
                }
                if (pc.Is(RoleEnum.Swapper))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#66E666FF>Swapper</color>, you can swap the votes of two players in meeting.");
                }
                if (pc.Is(RoleEnum.Transporter))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#00EEFFFF>Transporter</color>, you can swap the location of two players.");
                }
                if (pc.Is(RoleEnum.Informant))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#4CFAAB>Informant</color>, you can access vitals and admin from anywhere on the map.");
                }
                if (pc.Is(RoleEnum.Politician))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#660099FF>Politician</color>, you can use your campaign abilities to get voters to be elected as the new mayor.\nYou can also use the reveal button in meeting to reveal yourself as the new mayor but if less than half of the players in the lobby voted for you, then the campaign will fail and you won't be able to campaign for one round.\nNote that only Crewmate roles will count as voters.");
                }
                // Neutrals Roles infos
                if (pc.Is(RoleEnum.Amnesiac))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#80B2FFFF>Amnesiac</color>, you forgot your role so remember a dead player's one!");
                }
                if (pc.Is(RoleEnum.Shifter))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#AAAAAA>Shifter</color>, you can shift your role with anyone and you are not guessable!");
                }
                if (pc.Is(RoleEnum.GuardianAngel))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#B3FFFFFF>Guardian Angel</color>, you have to protect your target at any cost!\nYou can use your ability to give a shield to your target.\nIf your target dies before you, you role will change depending on the settings.");
                }
                if (pc.Is(RoleEnum.Survivor))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FFE64DFF>Survivor</color>, you can use your vest ability to give you a temporarily shield and become immune to attacks.\nYour goal is to stay alive until the end!");
                }
                if (pc.Is(RoleEnum.Doomsayer))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#00FF80FF>Doomsayer</color>, you can guess the role of other players in meeting. Guessing a certain amount of roles will result in making you win!");
                }
                if (pc.Is(RoleEnum.Executioner))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#8C4005FF>Executioner</color>, if you manage to eject your target, you win.");
                }
                if (pc.Is(RoleEnum.Jester))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FFBFCCFF>Jester</color>, get voted out to win!\nYou may be able to vent to act more suspicious.");
                }
                if (pc.Is(RoleEnum.SoulCollector))
                {
                    HudManager.Instance.ShowPopUp($"You are the <color=#99FFCCFF>Soul Collector</color>, your goal is to collect {CustomGameOptions.SoulsToWin} Souls to win.\nYou can reap players to collect more souls faster.");
                }
                if (pc.Is(RoleEnum.Vulture))
                {
                    HudManager.Instance.ShowPopUp($"You are the <color=#916e6e>Vulture</color>, eat {CustomGameOptions.VultureBodies} Dead Bodies to win!");
                }
                if (pc.Is(RoleEnum.Phantom))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#662962FF>Phantom</color>, finish your tasks before getting clicked to win.");
                }
                if (pc.Is(RoleEnum.Troll))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#c27d5a>Troll</color>, you can force someone to kill you, if this player gets ejected, you win.");
                }
                if (pc.Is(RoleEnum.Arsonist))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF4D00FF>Arsonist</color>, you can douse players and then kill all doused players by pressing the Ignite button.");
                }
                if (pc.Is(RoleEnum.Mutant))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#b6eb5e>Mutant</color>, you can kill normally but with a long cooldown and a low vison.\nYou can also decide to transform which will give you a shorter kill cooldown, an Impostor vision and also depending on settings, the ability to vent.");
                }
                if (pc.Is(RoleEnum.Infectious))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#bf9000>Infectious</color>, you can kill or infect player.\nInfecting a player will after each round give them a disadvantage until it kills them.\nOn stage 1 of the infection, nothing happens, on stage 2, they become slower, on stage 3, their vision gets reduced and on stage 4, they die.\nIf you kill an infected player, your next cooldown will be shorter.");
                }
                if (pc.Is(RoleEnum.Plaguebearer))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#E6FFB3FF>Plaguebearer</color>, infect everyone to turn into Pestilence!");
                }
                if (pc.Is(RoleEnum.Attacker))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#c73a00>Attacker</color>, finish your tasks to become <color=#8bce23>Terrorist</color>!");
                }
                if (pc.Is(RoleEnum.Terrorist))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#8bce23>Terrorist</color>, kill everyone to win!\nYou can not be guessed or voted out!");
                }
                if (pc.Is(RoleEnum.Pestilence))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#4c4c4c>Pestilence</color>, the most powerful role, no one can attack or guess you, the only way to get rid of you is by ejecting you.");
                }
                if (pc.Is(RoleEnum.SerialKiller))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#0c20d5>Serial Killer</color>, you can kill anyone and may also be able to convert someone to your side, depending on settings.");
                }
                if (pc.Is(RoleEnum.Doppelganger))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#fadd84>Doppelganger</color>, whenever you kill someone, you transform into them.\nNote that you can't transform with the reviver.\nKill everyone to win.");
                }
                if (pc.Is(RoleEnum.Glitch))
                {
                    HudManager.Instance.ShowPopUp("You are The <color=#00FF00FF>Glitch</color>, you can mimic to morph into someone else, hack a player to make them unable to use their abilities and kill anyone.");
                }
                if (pc.Is(RoleEnum.Vampire))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#262626FF>Vampire</color>, you can bite someone to convert them to your side, if the convert is unsuccessful, your cooldown will be reset.\nYou can also perform regular kills.");
                }
                if (pc.Is(RoleEnum.Maul))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#A86629FF>Maul</color>, use your Rampage ability to kill with a shorter kill cooldown and to get an Impostor vision.\nYou may also gain the ability to vent while rampaging, depending on settings.");
                }
                if (pc.Is(RoleEnum.Juggernaut))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#8c004c>Juggernaut</color>, when you use your kill ability, your next kill cooldown decreases etc until 0.");
                }
                // Impostors Roles info
                if (pc.Is(RoleEnum.Spirit))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Spirit</color>, if you manage to complete all your tasks before getting clicked, you will kill a random non impostor!");
                }
                if (pc.Is(RoleEnum.Freezer))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Freezer</color>, you can use your ability to temporarily make alive players unable to move.");
                }
                if (pc.Is(RoleEnum.Blinder))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Blinder</color>, you can use your ability to temporarily make alive players unable to see.");
                }
                if (pc.Is(RoleEnum.Escapist))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Escapist</color>, you can use your Mark button to set a point on the map, and then you can teleport to that point using the Recall button.");
                }
                if (pc.Is(RoleEnum.Grenadier))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Grenadier</color>, you can use your Flash button to make near Crewmates totally blind.");
                }
                if (pc.Is(RoleEnum.Morphling))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Morphling</color>, you can morph into the player you want after taking a sample of his body.");
                }
                if (pc.Is(RoleEnum.Hypnotist))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Hypnotist</color>, you can use your ability to hypnotize other players.\nOnce enough people are hypnotized, you can release Mass Hysteria making hypnotized players see all other players as either themselves, camouflaged or invisible.");
                }
                if (pc.Is(RoleEnum.Assassin))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Assassin</color>, you can guess the roles of players during the meeting to kill them.\nHowever, if your guess is wrong, you die instead.");
                }
                if (pc.Is(RoleEnum.Swooper))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Swooper</color>, you can turn invisible to kill without getting caught.");
                }
                if (pc.Is(RoleEnum.Venerer))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Venerer</color>, you unlock different abilities after each kill.");
                }
                if (pc.Is(RoleEnum.Poisoner))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Poisoner</color>, you can poison your target to make them die after a certain delay.");
                }
                if (pc.Is(RoleEnum.Shooter))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Shooter</color>, you can store bullets to kill again without cooldown after. Storing a bullet will reset your kill cooldown.");
                }
                if (pc.Is(RoleEnum.Bomber))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Bomber</color>, you can place a bomb somewhere on the map which will kill every player in its radius after exploding.\nBe careful because the bomb may also kill yourself or your teammates!");
                }
                if (pc.Is(RoleEnum.Traitor))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Traitor</color>, you betrayed the Crew and turned <color=#FF0000FF>Impostor</color>, now kill them all!");
                }
                if (pc.Is(RoleEnum.Warlock))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Warlock</color>, you can charge up your kill cooldown resulting in killing multiple players at the same time.");
                }
                if (pc.Is(RoleEnum.Mafioso))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Mafioso</color>, you can not kill, sabotage or vent until all of your teammates are dead.");
                }
                if (pc.Is(RoleEnum.Reviver))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Reviver</color>, if you die, you are able to revive inside someone else's body during the same round.\nBut you won't be able to kill unless all of your teamates are dead.\nIf the round ends and you didn't revive, then you won't be able to revive anymore.\nNote that you can't revive into all bodies (ex: bodies that switched identity with Doppelganger).");
                }
                if (pc.Is(RoleEnum.Blackmailer))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Blackmailer</color>, you can use your ability to make a player unable to talk in meetings!");
                }
                if (pc.Is(RoleEnum.Converter))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Converter</color>, you can use your ability revive a dead Crewmate and convert them to your side.");
                }
                if (pc.Is(RoleEnum.Janitor))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Janitor</color>, you can clean up dead bodies on the map.");
                }
                if (pc.Is(RoleEnum.Witch))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Witch</color>, you can cast spells on players, killing them after the next meeting.\nHowever, if you die before the end of that meeting or get ejected, the players won't die.\nYou can also perform regular kills.");
                }
                if (pc.Is(RoleEnum.Conjurer))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Conjurer</color>, you can curse a player and control him to kill with his identity.");
                }
                if (pc.Is(RoleEnum.BountyHunter))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Bounty Hunter</color>, a target is assigned to you, if you kill your target, you get a short kill cooldown, else a long.");
                }
                if (pc.Is(RoleEnum.Miner))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Miner</color>, you can place vents around the map and use them to go away after killing.");
                }
                if (pc.Is(RoleEnum.Undertaker))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Undertaker</color>, you can drag bodies around the map to hide them.");
                }
                if (pc.Is(RoleEnum.Manipulator))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FF0000FF>Manipulator</color>, you can use your Manipulate button to force the closest player to kill the next non-Impostor they find.\nNote that the Manipulation will be reset after a meeting.\nYou can also perform regular kills.");
                }
                //Coven Roles info
                if (pc.Is(RoleEnum.Coven))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#bf5fff>Coven</color>, you can kill & sabotage, kill all non-coven members to win!");
                }
                if (pc.Is(RoleEnum.Ritualist))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#bf5fff>Ritualist</color>, you can guess player's roles resulting in killing them.\nHowever, if you guess incorrectly, you will die instead!");
                }
                if (pc.Is(RoleEnum.HexMaster))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#bf5fff>Hex Master</color>, you can hex players and then hex bomb them all at the same time.\nHowever, everyone will know who the hexed players are.");
                }
                if (pc.Is(RoleEnum.CovenLeader))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#bf5fff>Coven Leader</color>, you have access to the vent and can convert anyone to coven once.");
                }
                if (pc.Is(RoleEnum.Spiritualist))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#bf5fff>Spiritualist</color>, you use your ability to control a player.\nIf this player tries to attack a coven members with a direct kill, the attack will reverse.\nThis does not apply to undirect kills (veteran, bomb, poison, guess...).");
                }
                if (pc.Is(RoleEnum.PotionMaster))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#bf5fff>Potion Master</color>, you use your ability to get a random potion effect.\nUsing it will give you one of the following potions:\nSpeed: Make your speed higher\nStrenght: You are able to vent and have a shorter kill cooldown\nShield: You are temporarily immune to direct kills\nInvisibility: You are invisible to other players");
                }
                //Werewolf Roles info
                if (pc.Is(RoleEnum.Werewolf))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#A86629FF>Werewolf</color>, kill all <color=#adf34b>Villagers</color> before they find your true identity.\nDuring the night, you can go on Rampage, making you able to kill.");
                }
                if (pc.Is(RoleEnum.Villager))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#adf34b>Villager</color>, you must exile the <color=#A86629FF>Werewolves</color> before they kill you.");
                }
                if (pc.Is(RoleEnum.Sorcerer))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#9210ff>Sorcerer</color>, during the night, you can Poison a player resulting in their death on the next Day.\nYou can also Revive a player.");
                }
                if (pc.Is(RoleEnum.Seer) && CustomGameOptions.GameMode == GameMode.Werewolf)
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FFCC80FF>Seer</color>, every night, you can reveal the role of a player.");
                }
                if (pc.Is(RoleEnum.Prosecutor) && CustomGameOptions.GameMode == GameMode.Werewolf)
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#B38000FF>Prosecutor</color>, you can choose to control the vote and vote someone out once.\nBe careful because if you exile a <color=#adf34b>Villager</color>, you will die.\nYou can not prosecute on the 10 last seconds of a meeting.");
                }
                if (pc.Is(RoleEnum.SoulCatcher))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#7b7781>Soul Catcher</color>, during the night, you will be able to see the Deads.\nBe careful, some might try to trick you!\nNote that you won't know who the Deads are.");
                }
                if (pc.Is(RoleEnum.WhiteWolf))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#ffffff>White Wolf</color>, you work the same way as a normal Werewolf, however you are not in the Werewolf team.\nYou must kill all <color=#A86629FF>Werewolves</color> and <color=#adf34b>Villagers</color> to win!");
                }
                if (pc.Is(RoleEnum.Guard))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#0095ff>Guard</color>, every night, you can Protect someone making them immune to <color=#A86629FF>Werewolves</color> attacks.\nProtection will be reset the next day.");
                }
                if (pc.Is(RoleEnum.Chameleon) && CustomGameOptions.GameMode == GameMode.Werewolf)
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#068c38>Chameleon</color>, during the night, you can turn invisible to watch other players without them knowing.");
                }
                if (pc.Is(RoleEnum.Mayor) && CustomGameOptions.GameMode == GameMode.Werewolf)
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#704FA8FF>Mayor</color>, during the day, you can choose to reveal which will safe you and gain you two extra votes.\nAdditionally, your vision will be reduced.");
                }
                if (pc.Is(RoleEnum.TalkativeWolf))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#ffa750>Talkative Wolf</color>, during the day, you will have to say a random word.\nIf you didn't say it before the day ends, you will die!");
                }
                if (pc.Is(RoleEnum.Sheriff) && CustomGameOptions.GameMode == GameMode.Werewolf)
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#FFFF00FF>Sheriff</color>, during the night, you can kill <color=#A86629FF>Werewolves</color> but if you try to kill <color=#adf34b>Villagers</color>, you will die!");
                }
                if (pc.Is(RoleEnum.BlackWolf))
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#4c4c4c>Black Wolf</color>, during the night, you can Convert a <color=#adf34b>Villager</color> to a <color=#A86629FF>Werewolf</color>!");
                }
                if (pc.Is(RoleEnum.Paranoïac) && CustomGameOptions.GameMode == GameMode.Werewolf)
                {
                    HudManager.Instance.ShowPopUp("You are the <color=#0a7eae>Paranoïac</color>, during the night, if you feel stressed, you can start the next day quicker and skip the current night once.\nYou can also hide in vents.");
                }
                //Battle Royale Role
                if (pc.Is(RoleEnum.Player))
                {
                    HudManager.Instance.ShowPopUp("This is <color=#006aff>Battle Royale</color> Mode, as a <color=#006aff>Player</color> your goal is to kill everyone and be the last one standing!\nNo meetings and report allowed.");
                }
            }
        }
    }
}