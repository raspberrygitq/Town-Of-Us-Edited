using HarmonyLib;
using System;
using System.Linq;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.WatcherMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Watcher)) return;
            var watcherRole = Role.GetRole<Watcher>(PlayerControl.LocalPlayer);
            foreach (var (key, value) in watcherRole.Watching)
            {
                var name = Utils.PlayerById(key).Data.PlayerName;
                if (value.Count == 0)
                {
                    if (HudManager.Instance)
                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"No players interacted with {name}");
                }
                else
                {
                    string message = $"Roles seen interacting with {name}:\n";
                    foreach (RoleEnum role in value.OrderBy(x => Guid.NewGuid()))
                    {
                        message += $" {role},";
                    }
                    message = message.Remove(message.Length - 1, 1);
                    if (HudManager.Instance)
                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
                }
            }
        }
    }
}
