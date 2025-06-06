using System;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUsEdited.Modifiers.AssassinMod;
using TownOfUsEdited.Roles;
using TownOfUsEdited.Roles.Modifiers;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Assassin = TownOfUsEdited.Roles.Modifiers.Assassin;
using TownOfUsEdited.CrewmateRoles.VigilanteMod;
using TownOfUsEdited.NeutralRoles.DoomsayerMod;
using Assassin2 = TownOfUsEdited.Roles.Assassin;
using TownOfUsEdited.CovenRoles.RitualistMod;

namespace TownOfUsEdited.CrewmateRoles.MayorMod
{
    public class AddRevealButton
    {
        public static Sprite RevealSprite => TownOfUsEdited.RevealSprite;

        public static void GenButton(Mayor role, int index)
        {
            var confirmButton = MeetingHud.Instance.playerStates[index].Buttons.transform.GetChild(0).gameObject;

            var newButton = Object.Instantiate(confirmButton, MeetingHud.Instance.playerStates[index].transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            renderer.sprite = RevealSprite;
            newButton.transform.position = confirmButton.transform.position - new Vector3(0.75f, 0f, 0f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(Reveal(role));
            role.RevealButton = newButton;
        }


        private static Action Reveal(Mayor role)
        {
            void Listener()
            {
                role.RevealButton.Destroy();
                role.Revealed = true;
                Utils.Rpc(CustomRPC.Reveal, role.Player.PlayerId);
            }

            return Listener;
        }

        public static void RemoveAssassin(Mayor mayor)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == mayor.Player.PlayerId);
            if (PlayerControl.LocalPlayer.Is(AbilityEnum.Assassin))
            {
                var assassin = Ability.GetAbility<Assassin>(PlayerControl.LocalPlayer);
                ShowHideButtons.HideTarget(assassin, voteArea.TargetPlayerId);
                voteArea.NameText.transform.localPosition = new Vector3(0.3384f, 0.0311f, -0.1f);
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Ritualist))
            {
                var rit = Role.GetRole<Ritualist>(PlayerControl.LocalPlayer);
                ShowHideButtonsRitualist.HideTarget(rit, voteArea.TargetPlayerId);
                voteArea.NameText.transform.localPosition = new Vector3(0.3384f, 0.0311f, -0.1f);
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer))
            {
                var doomsayer = Role.GetRole<Doomsayer>(PlayerControl.LocalPlayer);
                ShowHideButtonsDoom.HideTarget(doomsayer, voteArea.TargetPlayerId);
                voteArea.NameText.transform.localPosition = new Vector3(0.3384f, 0.0311f, -0.1f);
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante))
            {
                var vigilante = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer);
                ShowHideButtonsVigi.HideTarget(vigilante, voteArea.TargetPlayerId);
                voteArea.NameText.transform.localPosition = new Vector3(0.3384f, 0.0311f, -0.1f);
            }
            return;
        }

        public static void AddMayorButtons(MeetingHud __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Mayor))
            {
                var mayor = (Mayor)role;
                mayor.RevealButton.Destroy();
            }

            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor)) return;
            var mayorrole = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
            if (mayorrole.Revealed) return;
            for (var i = 0; i < __instance.playerStates.Length; i++)
                if (PlayerControl.LocalPlayer.PlayerId == __instance.playerStates[i].TargetPlayerId)
                {
                    GenButton(mayorrole, i);
                }
        }
    }
}