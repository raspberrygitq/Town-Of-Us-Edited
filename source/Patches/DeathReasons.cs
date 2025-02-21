using HarmonyLib;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class DeathReasonsShow
    {
        public static void UpdateMeeting(MeetingHud __instance)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) continue;
                    if (player.Data.IsDead && ((Utils.ShowDeadBodies && CustomGameOptions.DeadSeeRoles) || PlayerControl.LocalPlayer.Is(ModifierEnum.Scientist)))
                    {
                        var role = Role.GetRole(player);
                        if (role.DeathReason == DeathReasons.Misfired) state.NameText.text = state.NameText.text + "<color=#FFFF00FF><size=2>\n(Misfired)</size></color>";
                        else if (role.DeathReason == DeathReasons.Suicided) state.NameText.text = state.NameText.text + "<color=#FF0000><size=2>\n(Suicided)</size></color>";
                        else if (role.DeathReason == DeathReasons.Executed) state.NameText.text = state.NameText.text + "<color=#FF0000><size=2>\n(Executed)</size></color>";
                        else if (role.DeathReason == DeathReasons.Guessed) state.NameText.text = state.NameText.text + "<color=#FF0000><size=2>\n(Guessed)</size></color>";
                        else if (role.DeathReason == DeathReasons.Poisoned) state.NameText.text = state.NameText.text + "<color=#A020F0><size=2>\n(Poisoned)</size></color>";
                        else if (role.DeathReason == DeathReasons.Burned) state.NameText.text = state.NameText.text + "<color=#FF4D00FF><size=2>\n(Burned)</size></color>";
                        else if (role.DeathReason == DeathReasons.Hexed) state.NameText.text = state.NameText.text + "<color=#bf5fff><size=2>\n(Hexed)</size></color>";
                        else if (role.DeathReason == DeathReasons.Infected) state.NameText.text = state.NameText.text + "<color=#bf9000><size=2>\n(Infected)</size></color>";
                        else if (role.DeathReason == DeathReasons.Cursed) state.NameText.text = state.NameText.text + "<color=#FF0000><size=2>\n(Cursed)</size></color>";
                        else if (role.DeathReason == DeathReasons.Exploded) state.NameText.text = state.NameText.text + "<color=#FF0000><size=2>\n(Exploded)</size></color>";
                        else if (role.DeathReason == DeathReasons.Won) state.NameText.text = state.NameText.text + "<color=#00FFFF><size=2>\n(Won)</size></color>";
                        else if (role.DeathReason == DeathReasons.Killed) state.NameText.text = state.NameText.text + "<color=#FF0000><size=2>\n(Killed)</size></color>";
                        else if (role.DeathReason == DeathReasons.Exiled) state.NameText.text = state.NameText.text + "<color=#FF0000><size=2>\n(Exiled)</size></color>";
                        else if (role.DeathReason == DeathReasons.Spectator) state.NameText.text = state.NameText.text + "<color=#da6f00><size=2>\n(Spectator)</size></color>";
                    }
                }
            }
        }

        public static void Postfix()
        {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.IsDead && Utils.ShowDeadBodies && CustomGameOptions.DeadSeeRoles)
                    {
                        var role = Role.GetRole(player);
                        if (role.DeathReason == DeathReasons.Misfired) player.nameText().text = player.nameText().text + "<color=#FFFF00FF><size=2>\n(Misfired)</size></color>";
                        else if (role.DeathReason == DeathReasons.Suicided) player.nameText().text = player.nameText().text + "<color=#FF0000><size=2>\n(Suicided)</size></color>";
                        else if (role.DeathReason == DeathReasons.Executed) player.nameText().text = player.nameText().text + "<color=#FF0000><size=2>\n(Executed)</size></color>";
                        else if (role.DeathReason == DeathReasons.Guessed) player.nameText().text = player.nameText().text + "<color=#FF0000><size=2>\n(Guessed)</size></color>";
                        else if (role.DeathReason == DeathReasons.Poisoned) player.nameText().text = player.nameText().text + "<color=#A020F0><size=2>\n(Poisoned)</size></color>";
                        else if (role.DeathReason == DeathReasons.Burned) player.nameText().text = player.nameText().text + "<color=#FF4D00FF><size=2>\n(Burned)</size></color>";
                        else if (role.DeathReason == DeathReasons.Hexed) player.nameText().text = player.nameText().text + "<color=#bf5fff><size=2>\n(Hexed)</size></color>";
                        else if (role.DeathReason == DeathReasons.Infected) player.nameText().text = player.nameText().text + "<color=#bf9000><size=2>\n(Infected)</size></color>";
                        else if (role.DeathReason == DeathReasons.Cursed) player.nameText().text = player.nameText().text + "<color=#FF0000><size=2>\n(Cursed)</size></color>";
                        else if (role.DeathReason == DeathReasons.Exploded) player.nameText().text = player.nameText().text + "<color=#FF0000><size=2>\n(Exploded)</size></color>";
                        else if (role.DeathReason == DeathReasons.Won) player.nameText().text = player.nameText().text + "<color=#00FFFF><size=2>\n(Won)</size></color>";
                        else if (role.DeathReason == DeathReasons.Killed) player.nameText().text = player.nameText().text + "<color=#FF0000><size=2>\n(Killed)</size></color>";
                        else if (role.DeathReason == DeathReasons.Exiled) player.nameText().text = player.nameText().text + "<color=#FF0000><size=2>\n(Exiled)</size></color>";
                        else if (role.DeathReason == DeathReasons.Spectator) player.nameText().text = player.nameText().text + "<color=#da6f00><size=2>\n(Spectator)</size></color>";
                    }
                } 
            if (!MeetingHud.Instance) return;
            UpdateMeeting(MeetingHud.Instance);
        }
    }
}