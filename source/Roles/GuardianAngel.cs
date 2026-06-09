using TMPro;
using TownOfUsEdited.Patches;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class GuardianAngel : Role
    {
        public bool Enabled;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public float TimeRemaining;

        public int UsesLeft;
        public TextMeshPro UsesText;

        public bool ButtonUsable => UsesLeft != 0;

        public PlayerControl target;

        public GuardianAngel(PlayerControl player) : base(player)
        {
            Name = "Guardian Angel";
            ImpostorText = () =>
                target == null ? "You don't have a target for some reason... weird..." : $"Protect {target.name} With Your Life!";
            TaskText = () =>
                target == null
                    ? "You don't have a target for some reason... weird..."
                    : $"Protect {target.name}!";
            Color = Patches.Colors.GuardianAngel;
            Cooldown = CustomGameOptions.ProtectCd;
            RoleType = RoleEnum.GuardianAngel;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralBenign;
            Scale = 1.4f;

            UsesLeft = CustomGameOptions.MaxProtects;
        }

        public bool Protecting => TimeRemaining > 0f;

        public float ProtectTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }

        public void Protect()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }


        public void UnProtect()
        {
            TimeRemaining = 0f;
            ShowShield.ResetVisor(target, Player);
            Enabled = false;
            Cooldown = CustomGameOptions.ProtectCd;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var gaTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            gaTeam.Add(PlayerControl.LocalPlayer);
            gaTeam.Add(target);
            __instance.teamToShow = gaTeam;
        }
    }
}