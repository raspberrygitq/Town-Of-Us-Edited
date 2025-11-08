using System.Linq;
using HarmonyLib;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CovenRoles.HexMasterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ShowHexed
    {
        public static void UpdateMeeting(HexMaster role, MeetingHud __instance)
        {
            var hexed = role.Hexed;
            if (!role.Player.Data.IsDead && hexed != null)
            foreach (PlayerVoteArea pva in __instance.playerStates)
            {
                if (hexed.Any(x => x == pva.TargetPlayerId))
                {
                    var player = Utils.PlayerById(pva.TargetPlayerId);
                    if (!player.Data.IsDead)
                    pva.NameText.text = "<color=#bf5fff>乂 </color>" + pva.NameText.text;
                }
            }
        }

        public static void Postfix(HudManager __instance)
        {
            foreach (var hexmaster in Role.GetRoles(RoleEnum.HexMaster))
            {
                var role = Role.GetRole<HexMaster>(hexmaster.Player);
                var hexed = role.Hexed;
                if (role.Hexed == null) return;
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (hexed.Any(x => x == player.PlayerId) && (PlayerControl.LocalPlayer.Is(Faction.Coven) ||
                    PlayerControl.LocalPlayer.Data.IsDead) && !hexmaster.Player.Data.IsDead && !player.Data.IsDead)
                    {
                        player.nameText().text = "<color=#bf5fff>乂 </color>" + player.nameText().text;
                    }
                } 
            }  
            if (!MeetingHud.Instance) return;
            foreach (var hexmaster in Role.GetRoles(RoleEnum.HexMaster))
            {
                var role = Role.GetRole<HexMaster>(hexmaster.Player);
                if (role.Player.Data.IsDead || role.Hexed == null) return;
                UpdateMeeting(role, MeetingHud.Instance);
            }
        }
    }
}