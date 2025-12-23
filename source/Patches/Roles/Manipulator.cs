using TMPro;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Manipulator : Role

    {
        public KillButton _manipulateButton;
        public PlayerControl ClosestPlayer;
        public PlayerControl ManipulatedPlayer;
        public bool IsManipulating = false;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public bool Enabled;
        public float TimeRemaining;
        public bool UsingManipulation => TimeRemaining > 0f;

        public Manipulator(PlayerControl player) : base(player)
        {
            Name = "Manipulator";
            ImpostorText = () => "Manipulate The Crewmates";
            TaskText = () => "Use your ability to control <color=#00FFFF>Crewmates</color>\nFake Tasks:";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Manipulator;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            Alignment = Alignment.ImpostorKilling;
            Cooldown = CustomGameOptions.ManipulateCD;
        }
        public KillButton ManipulateButton
        {
            get => _manipulateButton;
            set
            {
                _manipulateButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void StartManipulation()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }


        public void StopManipulation()
        {
            if (ManipulatedPlayer == null) return;
            Enabled = false;
            Cooldown = CustomGameOptions.ManipulateCD;
            TimeRemaining = 0f;
            IsManipulating = false;
            if (PlayerControl.LocalPlayer == Player)
            {
                PlayerControl.LocalPlayer.moveable = true;
                Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(PlayerControl.LocalPlayer);
                var light = PlayerControl.LocalPlayer.lightSource;
                light.transform.SetParent(PlayerControl.LocalPlayer.transform);
                light.transform.localPosition = PlayerControl.LocalPlayer.Collider.offset;
            }
            else if (PlayerControl.LocalPlayer == ManipulatedPlayer)
            {
                PlayerControl.LocalPlayer.moveable = true;
                PlayerControl.LocalPlayer.NetTransform.Halt();
                HudManager.Instance.TaskCompleteOverlay.gameObject.SetActive(false);
                HudManager.Instance.TaskCompleteOverlay.gameObject.GetComponentInChildren<TextMeshPro>().text = TranslationController.Instance.GetString(StringNames.TaskComplete);
            }
            ManipulatedPlayer = null;
        }

        public float ManipulateTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }
    }
}