using System;
using TownOfUsEdited.Extensions;
using static TownOfUsEdited.Roles.Role;

namespace TownOfUsEdited.Roles.Modifiers
{
    public class Celebrity : Modifier
    {
        public bool JustDied = false;
        public bool MessageShown = false;
        public DateTime DeathTime { get; set; }
        public string Message = string.Empty;
        public string Room = string.Empty;
        public Celebrity(PlayerControl player) : base(player)
        {
            Name = "Celebrity";
            TaskText = () => "Announce how you died on your passing";
            Color = Patches.Colors.Celebrity;
            ModifierType = ModifierEnum.Celebrity;
        }

        public void GenMessage(PlayerControl killer)
        {
            JustDied = true;
            DeathTime = DateTime.UtcNow;
            Room = Room == string.Empty ? "Outside/Hallway" : Room;
            if (Player == killer) Message = $"The Celebrity, {Player.GetDefaultOutfit().PlayerName}, was killed!\nLocation: {Room}\nDeath: By Suicide\nTime: ";
            else Message = $"The Celebrity, {Player.GetDefaultOutfit().PlayerName}, was killed!\nLocation: {Room}\nDeath: By {GetRole(killer).Name.ToString()}\nTime: ";
            if (MeetingHud.Instance) PrintMessage();
        }

        public void PrintMessage()
        {
            var milliSeconds = (float) (DateTime.UtcNow - DeathTime).TotalMilliseconds;
            Message += $"{Math.Round(milliSeconds / 1000)} seconds ago.";
            MessageShown = true;
            Utils.Rpc(CustomRPC.CelebDied, Message);
        }
    }
}