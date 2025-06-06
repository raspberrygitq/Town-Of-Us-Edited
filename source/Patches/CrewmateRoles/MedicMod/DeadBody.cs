using System;
using System.Collections.Generic;
using TownOfUsEdited.Extensions;

namespace TownOfUsEdited.CrewmateRoles.MedicMod
{
    public class DeadPlayer
    {
        public byte KillerId { get; set; }
        public byte PlayerId { get; set; }
        public bool isDoppel { get; set; } = false;
        public DateTime KillTime { get; set; }
    }

    //body report class for when medic reports a body
    public class BodyReport
    {
        public PlayerControl Killer { get; set; }
        public PlayerControl Reporter { get; set; }
        public PlayerControl Body { get; set; }
        public float KillAge { get; set; }

        public static string ParseBodyReport(BodyReport br)
        {
            //System.Console.WriteLine(br.KillAge);
            if (br.KillAge > CustomGameOptions.MedicReportColorDuration * 1000)
                return
                    $"Body Report: The corpse is too old to gain information from. (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            if (br.Killer.PlayerId == br.Body.PlayerId)
                return
                    $"Body Report: The kill appears to have been a suicide! (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            var colors = new Dictionary<int, string>
            {
                {0, "darker"}, // Red
                {15, "darker"}, // Blue
                {22, "darker"}, // Green
                {6, "lighter"}, // Pink
                {2, "lighter"}, // Orange
                {31, "lighter"}, // Yellow
                {25, "darker"}, // Black
                {13, "lighter"}, // White
                {11, "darker"}, // Purple
                {24, "darker"}, // Brown
                {18, "lighter"}, // Cyan
                {33, "lighter"}, // Lime
                {9, "darker"}, // Maroon
                {30, "lighter"}, // Rose
                {29, "lighter"}, // Banana
                {14, "darker"}, // Grey
                {27, "darker"}, // Tan
                {7, "lighter"}, // Coral
                {8, "darker"}, // Melon
                {26, "darker"}, // Cocoa
                {16, "lighter"}, // Sky Blue
                {28, "lighter"}, // Biege
                {5, "darker"}, // Magenta
                {19, "lighter"}, // Aqua
                {12, "lighter"}, // Lilac
                {23, "darker"}, // Olive
                {17, "lighter"}, // Azure
                {10, "darker"}, // Plum
                {21, "darker"}, // Jungle
                {32, "lighter"}, // Mint
                {4, "lighter"}, // Lemon
                {20, "darker"}, // Macau
                {1, "darker"}, // Tawny
                {3, "lighter"}, // Gold
                {34, "lighter"}, // Rainbow
            };
            var typeOfColor = colors[br.Killer.GetDefaultOutfit().ColorId];
            return
                $"Body Report: The killer appears to be a {typeOfColor} color. (Killed {Math.Round(br.KillAge / 1000)}s ago)";
        }
    }
}
