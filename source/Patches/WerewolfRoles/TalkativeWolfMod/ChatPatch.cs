using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TownOfUs.Modifiers.AssassinMod;
using TownOfUs.Roles;

namespace TownOfUs.WerewolfRoles.TalkativeWolfMod.ChatPatch
{   
    public class ChatPatch
    {
        public static string Word = "";
        public static bool WordSaid = false;
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public class MeetingStart
        {
            public static void Postfix()
            {
                if (CustomGameOptions.GameMode != GameMode.Werewolf) return;
                var talkwolf = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.Disconnected && x.Is(RoleEnum.TalkativeWolf)).ToArray();
                if (!talkwolf.Any(x => !x.Data.Disconnected && !x.Data.IsDead)) return;

                WordSaid = false;
                Word = "";
                var impos = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.Disconnected && x.Is(Faction.Impostors)).ToArray();
                RandomWord();

                foreach (var player in impos)
                {
                    var playerResults = $"Today's word is {Word}, if the Talkative Wolf doesn't say it before the day ends, he will die!";
                    if (!string.IsNullOrWhiteSpace(playerResults) && player == PlayerControl.LocalPlayer) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(player, playerResults);
                }
            }

            public static void RandomWord()
            {
                var RandomWordList = new List<string> { "werewolf", "sus", "computer", "sister", "mayor", "die", "help", "killer", "plant", "time"
                , "heh", "impressive", "dragon", "fifty", "IQ", "monster", "L", "hardcore", "problem", "annoying"
                , "impossible", "neutral", "task", "kitchen", "eat", "cook", "miss", "smh", "hell", "cut"
                , "way", "talk", "storage", "sabotage", "defeat", "lose", "day", "night", "top", "tree"
                , "wish", "genius", "what", "technically", "apparently", "looks", "hold", "trust", "kill", "save"
                , "destroy", "repair", "fix", "imagine", "brave", "gorgeous", "hard", "easy", "middle", "come" };
                var random = new System.Random();
                int index = random.Next(RandomWordList.Count);
                var name = RandomWordList[index];
                RandomWordList.RemoveAt(index);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    Word = name;
                }
                else RandomWord();
            }
        }

        [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
        public class MeetingEnd
        {
            public static void Postfix()
            {
                foreach (var role in Role.GetRoles(RoleEnum.Prosecutor))
                {
                    var prosecutor = (Prosecutor)role;
                    if (prosecutor.ProsecuteThisMeeting) return;
                }
                if (PlayerControl.LocalPlayer.Data.IsDead) return;
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.TalkativeWolf)) return;
                if (WordSaid != true)
                {
                    AssassinKill.MurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                    Utils.Rpc(CustomRPC.TalkWolfDie, PlayerControl.LocalPlayer.PlayerId);
                }
                return;
            }
        }
    }
}