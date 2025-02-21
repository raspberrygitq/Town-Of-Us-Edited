using System.Linq;
using HarmonyLib;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.ImpostorRoles.WitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ShowCursed
    {
        public static void UpdateMeeting(Witch role, MeetingHud __instance)
        {
            var cursed = role.CursedList;
            if (!role.Player.Data.IsDead && cursed != null)
            foreach (PlayerVoteArea pva in __instance.playerStates)
            {
                if (cursed.Any(x => x == pva.TargetPlayerId))
                {
                    var player = Utils.PlayerById(pva.TargetPlayerId);
                    if (!player.Data.IsDead)
                    pva.NameText.text = "<color=#FF0000>† </color>" + pva.NameText.text;
                }
            }
        }

        public static void Postfix(HudManager __instance)
        {
            foreach (var witch in Role.GetRoles(RoleEnum.Witch))
            {
                var role = Role.GetRole<Witch>(witch.Player);
                var cursed = role.CursedList;
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (cursed.Any(x => x == player.PlayerId) && (PlayerControl.LocalPlayer.Is(Faction.Impostors) ||
                    PlayerControl.LocalPlayer.Data.IsDead) && !witch.Player.Data.IsDead && !player.Data.IsDead)
                    {
                        player.nameText().text = "<color=#FF0000>† </color>" + player.nameText().text;
                    }
                } 
            }  
            if (!MeetingHud.Instance) return;
            foreach (var witch in Role.GetRoles(RoleEnum.Witch))
            {
                var role = Role.GetRole<Witch>(witch.Player);
                if (role.Player.Data.IsDead || role.CursedList == null) return;
                UpdateMeeting(role, MeetingHud.Instance);
            }
        }
    }
}