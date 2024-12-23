using System;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Captain : Role
    {
        public Captain(PlayerControl owner) : base(owner)
        {
            Name = "Captain";
            ImpostorText = () => "Zoom The Map";
            TaskText = () => "Zoom out to catch the <color=#FF0000FF>Impostors</color>";
            Color = Patches.Colors.Captain;
            Cooldown = CustomGameOptions.ZoomCooldown;
            RoleType = RoleEnum.Captain;
            Alignment = Alignment.CrewmateInvestigative;
            AddToRoleHistory(RoleType);
            Faction = Faction.Crewmates;
        }
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public float TimeRemainingZoom;
        public bool ZoomEnabled;
        public bool Zooming => TimeRemainingZoom > 0f;
        public bool IsZooming;
        public void ZoomAbility()
        {
            IsZooming = !IsZooming;
            var size = CustomGameOptions.ZoomRange;
            Camera.main.orthographicSize = size;

            foreach (var cam in Camera.allCameras)
            {
            if (cam?.gameObject.name == "UI Camera")
            cam.orthographicSize = size;
            }

            ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);
            HudManager.Instance.ShadowQuad.gameObject.SetActive(false);

            ZoomEnabled = true;
            TimeRemainingZoom -= Time.deltaTime;
        }
        public void UnZoomAbility()
        {
            IsZooming = !IsZooming;
            var size = 3f;
            Camera.main.orthographicSize = size;

            foreach (var cam in Camera.allCameras)
            {
                if (cam?.gameObject.name == "UI Camera")
                    cam.orthographicSize = size;
            }

            ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);
    
            ZoomEnabled = false;
            Cooldown = CustomGameOptions.ZoomCooldown;
            if (!PlayerControl.LocalPlayer.Data.IsDead)
            HudManager.Instance.ShadowQuad.gameObject.SetActive(true);
        }
        public float ZoomTimer()
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