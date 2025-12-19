using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Roles;
using UnityEngine;
using UnityEngine.UI;

namespace TownOfUsEdited.NeutralRoles.DoomsayerMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
    public class ShowHideButtonsDoom
    {
        public static void HideButtonsDoom(Doomsayer role)
        {
            foreach (var (_, (cycleBack, cycleForward, guess, guessText)) in role.Buttons)
            {
                if (cycleBack == null || cycleForward == null) continue;
                cycleBack.SetActive(false);
                cycleForward.SetActive(false);
                guess.SetActive(false);
                guessText.gameObject.SetActive(false);

                cycleBack.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                cycleForward.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                guess.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            }
        }

        public static void HideSingle(
            Doomsayer role,
            byte targetId,
            bool killedSelf
        )
        {
            if (killedSelf) HideButtonsDoom(role);
            else HideTarget(role, targetId);
        }
        public static void HideTarget(
            Doomsayer role,
            byte targetId
        )
        {
            if (role.Buttons.ContainsKey(targetId))
            {
                var (cycleBack, cycleForward, guess, guessText) = role.Buttons[targetId];
                if (cycleBack == null || cycleForward == null) return;
                cycleBack.SetActive(false);
                cycleForward.SetActive(false);
                guess.SetActive(false);
                guessText.gameObject.SetActive(false);

                cycleBack.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                cycleForward.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                guess.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                role.Buttons[targetId] = (null, null, null, null);
                role.Guesses.Remove(targetId);

                PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                    x => x.TargetPlayerId == targetId);
                voteArea.NameText.transform.localPosition = new Vector3(0.3384f, 0.0311f, -0.1f);
            }
        }
    }
}
