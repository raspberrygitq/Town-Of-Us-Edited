using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CrewmateRoles.OracleMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HighlightConfessor
    {
        public static void UpdateMeeting(Oracle role, MeetingHud __instance)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                int accuracy = (int)role.Accuracy;
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) continue;
                    if (player == role.Confessor)
                    {
                        if (role.RevealedFaction == Faction.Crewmates && !role.Player.Is(Faction.Madmates)) state.NameText.text += $"<size=75%>{Palette.CrewmateBlue.ToTextColor()} ({accuracy}% Crew)</color></size>";
                        else if (role.RevealedFaction == Faction.Impostors && !role.Player.Is(Faction.Madmates)) state.NameText.text += $"<size=75%>{Palette.ImpostorRed.ToTextColor()} ({accuracy}% Imp)</color></size>";
                        else if (role.RevealedFaction == Faction.Madmates && !role.Player.Is(Faction.Madmates)) state.NameText.text += $"<size=75%>{Palette.ImpostorRed.ToTextColor()} ({accuracy}% Mad)</color></size>";
                        else if (role.RevealedFaction == Faction.Coven) state.NameText.text += $"<size=75%>{Patches.Colors.Coven.ToTextColor()} ({accuracy}% Coven)</color></size>";
                        else if (role.RevealedFaction == Faction.Impostors && role.Player.Is(Faction.Madmates)) state.NameText.text += $"<size=75%>{Palette.CrewmateBlue.ToTextColor()} ({accuracy}% Crew)</color></size>";
                        else if (role.RevealedFaction == Faction.Crewmates && role.Player.Is(Faction.Madmates)) state.NameText.text += $"<size=75%>{Palette.ImpostorRed.ToTextColor()} ({accuracy}% Imp)</color></size>";
                        else if (role.RevealedFaction == Faction.Madmates && role.Player.Is(Faction.Madmates)) state.NameText.text += $"<size=75%>{Palette.CrewmateBlue.ToTextColor()} ({accuracy}% Crew)</color></size>";
                        else state.NameText.text += $"<size=75%>{Color.gray.ToTextColor()} ({accuracy}% Neut)</color></size>";
                    }
                }
            }
        }
        public static void Postfix(HudManager __instance)
        {
            if (!MeetingHud.Instance || PlayerControl.LocalPlayer.Data.IsDead) return;
            foreach (var oracle in Role.GetRoles(RoleEnum.Oracle))
            {
                var role = Role.GetRole<Oracle>(oracle.Player);
                if (role == null || role.Player == null || role.Player.Data == null || role.Player.Data.Disconnected || !role.Player.Data.IsDead || role.Confessor == null) return;
                UpdateMeeting(role, MeetingHud.Instance);
            }
        }
    }
}