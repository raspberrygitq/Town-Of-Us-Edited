using TMPro;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Survivor : Role
    {
        public bool Enabled;
        public float TimeRemaining;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public int UsesLeft;
        public TextMeshPro UsesText;

        public bool ButtonUsable => UsesLeft != 0;
        public bool SpawnedAs = true;


        public Survivor(PlayerControl player) : base(player)
        {
            Name = "Survivor";
            ImpostorText = () => "Do Whatever It Takes To Live";
            TaskText = () => SpawnedAs ? "Stay alive to win" : "Your target was killed. Now you just need to live!";
            Color = Patches.Colors.Survivor;
            RoleType = RoleEnum.Survivor;
            Faction = Faction.NeutralBenign;
            AddToRoleHistory(RoleType);
            Cooldown = CustomGameOptions.VestCd;

            UsesLeft = CustomGameOptions.MaxVests;
        }

        public bool Vesting => TimeRemaining > 0f;

        public float VestTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }

        public void Vest()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }


        public void UnVest()
        {
            Enabled = false;
            Cooldown = CustomGameOptions.VestCd;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var survTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            survTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = survTeam;
        }

    }
}