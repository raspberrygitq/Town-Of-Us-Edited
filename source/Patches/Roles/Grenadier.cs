using System;
using UnityEngine;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
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
        static readonly Color dimVision = new Color(0.6f, 0.6f, 0.6f, 0.2f);
        static readonly Color blindVision = new Color(0.6f, 0.6f, 0.6f, 1f);
        public Il2CppSystem.Collections.Generic.List<PlayerControl> flashedPlayers = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

        public Grenadier(PlayerControl player) : base(player)
        {
            Name = "Grenadier";
            ImpostorText = () => "Hinder The Crewmates' Vision";
            TaskText = () => "Blind the <color=#00FFFF>Crewmates</color> to get sneaky kills\nFake Tasks:";
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
        public void Flash()
        {
            if (Enabled != true)
            {
                closestPlayers = Utils.GetClosestPlayers(Player.GetTruePosition(), CustomGameOptions.FlashRadius, true);
                flashedPlayers = closestPlayers;
            }
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            //To stop the scenario where the flash and sabotage are called at the same time.
            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            var sabActive = system.AnyActive;

            foreach (var player in closestPlayers)
            {
                if (PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
                {
                    if (TimeRemaining > CustomGameOptions.GrenadeDuration - 0.5f && (!sabActive))
                    {
                        float fade = (TimeRemaining - CustomGameOptions.GrenadeDuration) * -2.0f;
                        if (ShouldPlayerBeBlinded(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color.Lerp(normalVision, blindVision, fade);
                        }
                        else if (ShouldPlayerBeDimmed(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color.Lerp(normalVision, dimVision, fade);
                            if (PlayerControl.LocalPlayer.Data.IsImpostor() && MapBehaviour.Instance.infectedOverlay.sabSystem.Timer < 0.5f)
                            {
                                MapBehaviour.Instance.infectedOverlay.sabSystem.Timer = 0.5f;
                            }
                        }
                        else
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
                        }
                    }
                    else if (TimeRemaining <= (CustomGameOptions.GrenadeDuration - 0.5f) && TimeRemaining >= 0.5f && (!sabActive))
                    {
                        if (ShouldPlayerBeBlinded(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = blindVision;
                        }
                        else if (ShouldPlayerBeDimmed(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = dimVision;
                            if (PlayerControl.LocalPlayer.Data.IsImpostor() && MapBehaviour.Instance.infectedOverlay.sabSystem.Timer < 0.5f)
                            {
                                MapBehaviour.Instance.infectedOverlay.sabSystem.Timer = 0.5f;
                            }
                        }
                        else
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
                        }
                    }
                    else if (TimeRemaining < 0.5f && (!sabActive))
                    {
                        float fade2 = (TimeRemaining * -2.0f) + 1.0f;
                        if (ShouldPlayerBeBlinded(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color.Lerp(blindVision, normalVision, fade2);
                        }
                        else if (ShouldPlayerBeDimmed(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color.Lerp(dimVision, normalVision, fade2);
                            if (PlayerControl.LocalPlayer.Data.IsImpostor() && MapBehaviour.Instance.infectedOverlay.sabSystem.Timer < 0.5f)
                            {
                                MapBehaviour.Instance.infectedOverlay.sabSystem.Timer = 0.5f;
                            }
                        }
                        else
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
                        }
                    }
                    else
                    {
                        ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                        ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                        DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
                        TimeRemaining = 0.0f;
                    }
                }
            }

            if (TimeRemaining > 0.5f)
            {
                if (PlayerControl.LocalPlayer.Data.IsImpostor() && MapBehaviour.Instance.infectedOverlay.sabSystem.Timer < 0.5f)
                {
                    MapBehaviour.Instance.infectedOverlay.sabSystem.Timer = 0.5f;
                }
            }
        }

        private static bool ShouldPlayerBeDimmed(PlayerControl player) {
            return (player.Data.IsImpostor() || player.Data.IsDead) && !MeetingHud.Instance;
        }

        private static bool ShouldPlayerBeBlinded(PlayerControl player) {
            return !player.Data.IsImpostor() && !player.Data.IsDead && !MeetingHud.Instance;
        }

        public void UnFlash()
        {
            Enabled = false;
            Cooldown = CustomGameOptions.GrenadeCd;
            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
            DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
            flashedPlayers.Clear();
        }
    }
}