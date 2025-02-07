using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;

namespace TownOfUs.Patches.Modifiers.MotionlessMod
{
    // Code from stellar roles, link: https://github.com/Mr-Fluuff/StellarRolesAU/blob/bad6c0e70557897021fc9b257588b32e29b705b9/StellarRoles/Patches/SleepwalkerPatches.cs
    public class PositionPatches
    {
        // Save the position of the player prior to starting the climb / gap platform
        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
        public class FixLadder
        {
            public static void Prefix()
            {
                if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Motionless)) return;
                var role = Modifier.GetModifier<Motionless>(PlayerControl.LocalPlayer);
                role.Position = PlayerControl.LocalPlayer.transform.position;
                if (PlayerControl.LocalPlayer.inVent)
                {
                    role.inVent = true;
                    role.VentId = Vent.currentVent.Id;
                }
            }
        }

        [HarmonyPatch(typeof(PlatformConsole), nameof(PlatformConsole.Use))]
        public class FixPlatform
        {
            public static void Prefix()
            {
                if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Motionless)) return;
                var role = Modifier.GetModifier<Motionless>(PlayerControl.LocalPlayer);
                role.Position = PlayerControl.LocalPlayer.transform.position;
                if (PlayerControl.LocalPlayer.inVent)
                {
                    role.inVent = true;
                    role.VentId = Vent.currentVent.Id;
                }
            }
        }

        [HarmonyPatch(typeof(ZiplineBehaviour), nameof(ZiplineBehaviour.Use), typeof(PlayerControl), typeof(bool))]
        public class FixZipline
        {
            public static void Prefix()
            {
                if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Motionless)) return;
                var role = Modifier.GetModifier<Motionless>(PlayerControl.LocalPlayer);
                role.Position = PlayerControl.LocalPlayer.transform.position;
                if (PlayerControl.LocalPlayer.inVent)
                {
                    role.inVent = true;
                    role.VentId = Vent.currentVent.Id;
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
        class StartMeetingPatch
        {
            public static void Prefix()
            {
                bool Manipulated = false;
                var manipulator = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.Is(RoleEnum.Manipulator) && Role.GetRole<Manipulator>(x).ManipulatedPlayer == PlayerControl.LocalPlayer);
                if (PlayerControl.LocalPlayer.IsManipulated() && manipulator != null)
                {
                    var manipRole = Role.GetRole<Manipulator>(manipulator);
                    if (manipRole.UsingManipulation) Manipulated = true;
                }
                // Save Motionless position, if the player is able to move (i.e. not on a ladder or a gap thingy)
                if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Motionless)) return;
                var role = Modifier.GetModifier<Motionless>(PlayerControl.LocalPlayer);
                if (PlayerControl.LocalPlayer.MyPhysics.enabled && PlayerControl.LocalPlayer.moveable || PlayerControl.LocalPlayer.inVent
                || Manipulated || PlayerControl.LocalPlayer.IsFrozen() || Utils.Rewinding()) role.Position = PlayerControl.LocalPlayer.transform.position;
                if (PlayerControl.LocalPlayer.inVent)
                {
                    role.inVent = true;
                    role.VentId = Vent.currentVent.Id;
                }
            }
        }

        [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Close))]  // Set position of AntiTp players AFTER they have selected a spawn.
        class AirshipSpawnInPatch
        {
            static void Postfix()
            {
                if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Motionless)) return;
                var role = Modifier.GetModifier<Motionless>(PlayerControl.LocalPlayer);
                role.ResetPosition();
                if (PlayerControl.LocalPlayer.inVent)
                {
                    role.inVent = true;
                    role.VentId = Vent.currentVent.Id;
                }
            }
        }
    }
}