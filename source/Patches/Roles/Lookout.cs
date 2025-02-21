using TownOfUsEdited.Patches.CrewmateRoles.LookoutMod;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Lookout : Role

    {
        public PlayerControl ClosestPlayer;
        public PlayerControl WatchedPlayer;
        public bool IsWatching = false;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public bool Enabled;
        public float TimeRemaining;
        public bool UsingWatch => TimeRemaining > 0f;

        public Lookout(PlayerControl player) : base(player)
        {
            Name = "Lookout";
            ImpostorText = () => "Watch The Players";
            TaskText = () => "Use your ability to watch <color=#00FFFF>Crewmates</color>";
            Color = Patches.Colors.Lookout;
            RoleType = RoleEnum.Lookout;
            AddToRoleHistory(RoleType);
            Faction = Faction.Crewmates;
            Alignment = Alignment.CrewmateInvestigative;
            Cooldown = CustomGameOptions.WatchCD;
        }

        public void StartWatching()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }


        public void StopWatching()
        {
            if (WatchedPlayer == null) return;
            Enabled = false;
            Cooldown = CustomGameOptions.WatchCD;
            TimeRemaining = 0f;
            IsWatching = false;
            if (PlayerControl.LocalPlayer == Player)
            {
                PlayerControl.LocalPlayer.moveable = true;
                Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(PlayerControl.LocalPlayer);
                var light = PlayerControl.LocalPlayer.lightSource;
                light.transform.SetParent(PlayerControl.LocalPlayer.transform);
                light.transform.localPosition = PlayerControl.LocalPlayer.Collider.offset;
            }
            else if (PlayerControl.LocalPlayer == WatchedPlayer)
            {
                if (LookoutPatches.TaskOverlay != null)
                {
                    LookoutPatches.TaskOverlay.gameObject.SetActive(false);
                }
            }
            WatchedPlayer = null;
        }

        public float WatchTimer()
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