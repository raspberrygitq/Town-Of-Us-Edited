using UnityEngine;

namespace TownOfUsEdited.Patches
{
    class Colors {

        // Crew Colors
        public readonly static Color Crewmate = Palette.CrewmateBlue;
        public readonly static Color Mayor = new Color(0.44f, 0.31f, 0.66f, 1f);
        public readonly static Color Sheriff = Color.yellow;
        public readonly static Color Engineer = new Color(1f, 0.65f, 0.04f, 1f);
        public readonly static Color Swapper = new Color(0.4f, 0.9f, 0.4f, 1f);
        public readonly static Color Investigator = new Color(0f, 0.7f, 0.7f, 1f);
        public readonly static Color Medic = new Color(0f, 0.4f, 0f, 1f);
        public readonly static Color Seer = new Color(1f, 0.8f, 0.5f, 1f);
        public readonly static Color Spy = new Color(0.8f, 0.64f, 0.8f, 1f);
        public readonly static Color Snitch = new Color(0.83f, 0.69f, 0.22f, 1f);
        public readonly static Color Altruist = new Color(0.4f, 0f, 0f, 1f);
        public readonly static Color Vigilante = new Color(1f, 1f, 0.6f, 1f);
        public readonly static Color Veteran = new Color(0.6f, 0.5f, 0.25f, 1f);
        public readonly static Color Haunter = new Color(0.83f, 0.83f, 0.83f, 1f);
        public readonly static Color Tracker = new Color(0f, 0.6f, 0f, 1f);
        public readonly static Color Transporter = new Color(0f, 0.93f, 1f, 1f);
        public readonly static Color Medium = new Color(0.65f, 0.5f, 1f, 1f);
        public readonly static Color Mystic = new Color(0.3f, 0.6f, 0.9f, 1f);
        public readonly static Color Trapper = new Color(0.65f, 0.82f, 0.7f, 1f);
        public readonly static Color Detective = new Color(0.3f, 0.3f, 1f, 1f);
        public readonly static Color Chameleon = new Color32(6, 140, 56, 255);
        public readonly static Color Imitator = new Color(0.7f, 0.85f, 0.3f, 1f);
        public readonly static Color VampireHunter = new Color(0.7f, 0.7f, 0.9f, 1f);
        public readonly static Color Prosecutor = new Color(0.7f, 0.5f, 0f, 1f);
        public readonly static Color Oracle = new Color(0.75f, 0f, 0.75f, 1f);
        public readonly static Color Aurial = new Color(0.7f, 0.3f, 0.6f, 1f);
        public readonly static Color Hunter = new Color(0.16f, 0.67f, 0.53f, 1f);
        public readonly static Color Captain = new Color32(107, 141, 255, 255);
        public readonly static Color Doctor = new Color32(0, 238, 60, 255);
        public readonly static Color Knight = new Color32(255, 208, 130, 255);
        public readonly static Color Jailor = new Color32(97, 163, 122, 255);
        public readonly static Color Fighter = new Color32(147, 88, 230, 255);
        public readonly static Color Paranoïac = new Color32(10, 126, 174, 255);
        public readonly static Color Villager = new Color32(173, 243, 75, 255);
        public readonly static Color Sorcerer = new Color32(146, 16, 255, 255);
        public readonly static Color SoulCatcher = new Color32(123, 119, 129, 255);
        public readonly static Color WhiteWolf = Color.white;
        public readonly static Color Guard = new Color32(0, 149, 255, 255);
        public readonly static Color Avenger = new Color32(33, 111, 1, 255);
        public readonly static Color Astral = new Color32(167, 3, 174, 255);
        public readonly static Color Bodyguard = new Color32(37, 91, 201, 255);
        public readonly static Color Crusader = new Color32(129, 82, 48, 255);
        public readonly static Color Informant = new Color32(76, 250, 171, 255);
        public readonly static Color Deputy = new Color(1f, 0.8f, 0f, 1f);
        public readonly static Color TimeLord = new Color32(15, 0, 184, 255);
        public readonly static Color Helper = new Color32(124, 182, 194, 255);
        public readonly static Color Guardian = new Color32(103, 187, 67, 255);
        public readonly static Color Politician = new Color(0.4f, 0f, 0.6f, 1f);
        public readonly static Color Warden = new Color(0.6f, 0f, 1f, 1f);
        public readonly static Color Lookout = new Color(0.2f, 1f, 0.4f, 1f);
        public readonly static Color Plumber = new Color(0.8f, 0.4f, 0f, 1f);
        public readonly static Color Cleric = new Color(0f, 1f, 0.7f, 1f);

        // Neutral Colors
        public readonly static Color Jester = new Color(1f, 0.75f, 0.8f, 1f);
        public readonly static Color Executioner = new Color(0.55f, 0.25f, 0.02f, 1f);
        public readonly static Color Glitch = Color.green;
        public readonly static Color SerialKiller = new Color32(12, 32, 213, 255);
        public readonly static Color Mutant = new Color32(182, 235, 94, 255);
        public readonly static Color Shifter = new Color32(170, 170, 170, 255);
        public readonly static Color Troll = new Color32(194, 125, 90, 255);
        public readonly static Color Arsonist = new Color(1f, 0.3f, 0f);
        public readonly static Color Phantom = new Color(0.4f, 0.16f, 0.38f, 1f);
        public readonly static Color Amnesiac = new Color(0.5f, 0.7f, 1f, 1f);
        public readonly static Color Juggernaut = new Color(0.55f, 0f, 0.3f, 1f);
        public readonly static Color Survivor = new Color(1f, 0.9f, 0.3f, 1f);
        public readonly static Color GuardianAngel = new Color(0.7f, 1f, 1f, 1f);
        public readonly static Color Plaguebearer = new Color(0.9f, 1f, 0.7f, 1f);
        public readonly static Color Pestilence = new Color(0.3f, 0.3f, 0.3f, 1f);
        public readonly static Color Werewolf = new Color(0.66f, 0.4f, 0.16f, 1f);
        public readonly static Color Doomsayer = new Color(0f, 1f, 0.5f, 1f);
        public readonly static Color Vampire = new Color(0.15f, 0.15f, 0.15f, 1f);
        public readonly static Color Player = new Color32(0, 106, 255, 255);
        public readonly static Color Attacker = new Color32(199, 58, 0, 255);
        public readonly static Color Terrorist = new Color32(138, 206, 35, 255);
        public readonly static Color Vulture = new Color32(145, 110, 110, 255);
        public readonly static Color Infectious = new Color32(191, 144, 0, 255);
        public readonly static Color Doppelganger = new Color32(250, 221, 132, 255);
        public readonly static Color SoulCollector = new Color(0.6f, 1f, 0.8f, 1f);
        public readonly static Color Mercenary = new Color(0.55f, 0.4f, 0.6f, 1f);

        //Imposter Colors
        public readonly static Color Impostor = Palette.ImpostorRed;
        public readonly static Color BlackWolf = new Color32(76, 76, 76, 255);
        public readonly static Color TalkativeWolf = new Color32(255, 167, 80, 255);

        //Coven
        public readonly static Color Coven = new Color32(191, 95, 255, 255);

        // Spectator
        public readonly static Color Spectator = new Color32(218, 111, 0, 255);

        //Modifiers
        public readonly static Color Bait = new Color(0f, 0.7f, 0.7f, 1f);
        public readonly static Color Aftermath = new Color(0.65f, 1f, 0.65f, 1f);
        public readonly static Color Diseased = Color.grey;
        public readonly static Color Torch = new Color(1f, 1f, 0.6f, 1f);
        public readonly static Color ButtonBarry = new Color(0.9f, 0f, 1f, 1f);
        public readonly static Color Flash = new Color(1f, 0.5f, 0.5f, 1f);
        public readonly static Color Giant = new Color(1f, 0.7f, 0.3f, 1f);
        public readonly static Color Lovers = new Color(1f, 0.4f, 0.8f, 1f);
        public readonly static Color Sleuth = new Color(0.5f, 0.2f, 0.2f, 1f);
        public readonly static Color Tiebreaker = new Color(0.6f, 0.9f, 0.6f, 1f);
        public readonly static Color Radar = new Color(1f, 0f, 0.5f, 1f);
        public readonly static Color Multitasker = new Color(1f, 0.5f, 0.3f, 1f);
        public readonly static Color Scientist = new Color32(84, 65, 179, 255);
        public readonly static Color Frosty = new Color(0.6f, 1f, 1f, 1f);
        public readonly static Color SixthSense = new Color(0.85f, 1f, 0.55f, 1f);
        public readonly static Color Shy = new Color(1f, 0.7f, 0.8f, 1f);
        public readonly static Color Vengeful = new Color32(141, 0, 0, 255);
        public readonly static Color Spotter = new Color32(198, 136, 242, 255);
        public readonly static Color Motionless = new Color32(0, 133, 145, 255);
        public readonly static Color Mini = new Color(0.8f, 1f, 0.9f, 1f);
        public readonly static Color Taskmaster = new Color(0.4f, 0.6f, 0.4f, 1f);
        public readonly static Color Satellite = new Color(0f, 0.6f, 0.8f, 1f);
        public readonly static Color Superstar = new Color32(255, 202, 0, 255);
    }
}
