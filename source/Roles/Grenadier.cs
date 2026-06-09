using TownOfUsEdited.Extensions;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Grenadier : Role
    {
        public KillButton _flashButton;
        public bool Enabled;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public float TimeRemaining;
        public static Il2CppSystem.Collections.Generic.List<PlayerControl> closestPlayers = null;

        static readonly Color normalVision = new Color(0.6f, 0.6f, 0.6f, 0f);
        public Il2CppSystem.Collections.Generic.List<PlayerControl> flashedPlayers = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
        public float flashPercent = 0f;

        public Grenadier(PlayerControl player) : base(player)
        {
            Name = "Grenadier";
            ImpostorText = () => "Flashbang The Crewmates";
            TaskText = () => "Flash the <color=#00FFFF>Crewmates</color> to get sneaky kills\nFake Tasks:";
            Color = Patches.Colors.Impostor;
            Cooldown = CustomGameOptions.GrenadeCd;
            RoleType = RoleEnum.Grenadier;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            Alignment = Alignment.ImpostorConcealing;
        }

        public bool Flashed => TimeRemaining > 0f;


        public KillButton FlashButton
        {
            get => _flashButton;
            set
            {
                _flashButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float FlashTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }
        public void StartFlash()
        {
            closestPlayers = Utils.GetClosestPlayers(Player.GetTruePosition(), CustomGameOptions.FlashRadius, true);
            flashedPlayers = closestPlayers;
            Flash();
        }
        public void Flash()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            //To stop the scenario where the flash and sabotage are called at the same time.
            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            var sabActive = system.AnyActive;

            if (flashedPlayers.Contains(PlayerControl.LocalPlayer))
            {
                if (TimeRemaining > CustomGameOptions.GrenadeDuration - 0.5f && !sabActive && !MeetingHud.Instance)
                {
                    float fade = (TimeRemaining - CustomGameOptions.GrenadeDuration) * -2.0f;
                    flashPercent = fade;
                }
                else if (TimeRemaining <= (CustomGameOptions.GrenadeDuration - 0.5f) && TimeRemaining >= 0.5f && !sabActive && !MeetingHud.Instance)
                {
                    flashPercent = 1f;
                }
                else if (TimeRemaining < 0.5f && !sabActive && !MeetingHud.Instance)
                {
                    float fade = TimeRemaining * 2.0f;
                    flashPercent = fade;
                }
                else
                {
                    ((Renderer)HudManager.Instance.FullScreen).enabled = true;
                    ((Renderer)HudManager.Instance.FullScreen).gameObject.active = true;
                    HudManager.Instance.FullScreen.color = normalVision;
                    flashPercent = 0f;
                    TimeRemaining = 0.0f;
                }
            }

            if (TimeRemaining > 0.5f)
            {
                try
                {
                    if (PlayerControl.LocalPlayer.Data.IsImpostor() && MapBehaviour.Instance.infectedOverlay.sabSystem.Timer < 0.5f)
                    {
                        MapBehaviour.Instance.infectedOverlay.sabSystem.Timer = 0.5f;
                    }
                }
                catch { }
            }
        }

        public void UnFlash()
        {
            Enabled = false;
            flashPercent = 0f;
            Cooldown = CustomGameOptions.GrenadeCd;
            ((Renderer)HudManager.Instance.FullScreen).enabled = true;
            HudManager.Instance.FullScreen.color = normalVision;
            flashedPlayers.Clear();
        }
    }
}