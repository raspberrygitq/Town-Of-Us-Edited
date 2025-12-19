using HarmonyLib;
using Reactor.Utilities.Extensions;
using TownOfUsEdited.Patches;
using TownOfUsEdited.Patches.CovenRoles;
using TownOfUsEdited.Patches.CrewmateRoles.JailorMod;
using TownOfUsEdited.Patches.ImpostorRoles;
using TownOfUsEdited.Patches.Modifiers.LoversMod;
using TownOfUsEdited.Patches.NeutralRoles.SerialKillerMod;
using TownOfUsEdited.Patches.NeutralRoles.VampireMod;
using TownOfUsEdited.Roles;
using TownOfUsEdited.Roles.Modifiers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUsEdited
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingHud_Start
    {
        public static void Postfix(MeetingHud __instance)
        {
            ImpostorChat.UpdateImpostorChat();
            VampireChat.UpdateVampireChat();
            CovenChat.UpdateCovenChat();
            SerialKillerChat.UpdateSerialKillerChat();
            LoversChat.UpdateLoversChat();
            JailorChat.UpdateJailorChat();
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Astral))
            {
                var astral = Role.GetRole<Astral>(PlayerControl.LocalPlayer);
                Utils.ShowDeadBodies = PlayerControl.LocalPlayer.Data.IsDead && !astral.Enabled;
            }
            else
            {
                Utils.ShowDeadBodies = PlayerControl.LocalPlayer.Data.IsDead;
            }

            if (PlayerControl.LocalPlayer.Is(ModifierEnum.Bloodlust))
            {
                var modifier = Modifier.GetModifier<Bloodlust>(PlayerControl.LocalPlayer);
                modifier.KilledThisRound = 0;
            }

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                player.MyPhysics.ResetAnimState();
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Captain))
            {
                var captain = Role.GetRole<Captain>(PlayerControl.LocalPlayer);
                if (captain.Zooming)
                {
                    captain.TimeRemainingZoom = 0.01f;
                }
            }

            foreach (var role in Modifier.GetModifiers(ModifierEnum.Superstar))
            {
                var superstar = (Superstar)role;
                if (superstar.Player.Data.IsDead && superstar.Reported == false)
                {
                    superstar.Reported = true;
                }
            }

            HudUpdate.Zooming = false;
            Camera.main.orthographicSize = 3f;

            foreach (var cam in Camera.allCameras)
            {
                if (cam?.gameObject.name == "UI Camera")
                    cam.orthographicSize = 3f;
            }

            var targetRole = Role.GetRole(PlayerControl.LocalPlayer);
            if (targetRole != null) targetRole.RegenTask();

            ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);

            if (!PlayerControl.LocalPlayer.Data.IsDead) HudManager.Instance.ShadowQuad.gameObject.SetActive(true);
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    public class MeetingHud_Close
    {
        public static void Postfix(MeetingHud __instance)
        {
            Utils.Rpc(CustomRPC.RemoveAllBodies);
            var buggedBodies = Object.FindObjectsOfType<DeadBody>();
            foreach (var body in buggedBodies)
            {
                body.gameObject.Destroy();
            }
            var targetRole = Role.GetRole(PlayerControl.LocalPlayer);
            if (targetRole != null) targetRole.RegenTask();
        }
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
    public class ExileAnimStart
    {
        public static void Postfix(ExileController __instance, [HarmonyArgument(0)] NetworkedPlayerInfo exiled, [HarmonyArgument(1)] bool tie)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Astral))
            {
                var astral = Role.GetRole<Astral>(PlayerControl.LocalPlayer);
                Utils.ShowDeadBodies = (PlayerControl.LocalPlayer.Data.IsDead || exiled?.PlayerId == PlayerControl.LocalPlayer.PlayerId) && !astral.Enabled;
            }
            else
            {
                Utils.ShowDeadBodies = PlayerControl.LocalPlayer.Data.IsDead || exiled?.PlayerId == PlayerControl.LocalPlayer.PlayerId;
            }
        }
    }
}