using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine.UI;

namespace TownOfUsEdited.CrewmateRoles.DeputyMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
    public static class RemoveButtons
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Deputy))
            {
                var dep = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
                HideButtons(dep);
            }
        }

        public static void HideButtons(Deputy role)
        {
            foreach (var (_, button) in role.Buttons)
            {
                if (button == null) continue;
                button.SetActive(false);
                button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            }
            role.Buttons.Clear();
        }
    }
}