using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Shifter : Role
    {
        public bool SpawnedAs = true;
        public Shifter(PlayerControl player) : base(player)
        {
            Name = "Shifter";
            ImpostorText = () => "Shift your role";
            TaskText = () => SpawnedAs ? "Exchange your role with another player\nFake Tasks:" : "Your target was killed. Now shift your role!\nFake Tasks:";
            Color = Patches.Colors.Shifter;
            RoleType = RoleEnum.Shifter;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralBenign;
            Cooldown = CustomGameOptions.ShiftCD;
        }

        public PlayerControl ClosestPlayer;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var shifterTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            shifterTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = shifterTeam;
        }
        public float ShiftTimer()
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