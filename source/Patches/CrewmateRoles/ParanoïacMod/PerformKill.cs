using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.ParanoïacMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Paranoïac);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Paranoïac>(PlayerControl.LocalPlayer);
            if (!role.ButtonUsable) return false;
            var PanicButton = HudManager.Instance.KillButton;
            if (__instance == PanicButton)
            {
                if (!__instance.isActiveAndEnabled) return false;
                var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                if (!abilityUsed) return false;
                role.UsesLeft--;
                Utils.Rpc(CustomRPC.BarryButton, PlayerControl.LocalPlayer.PlayerId);

                if (AmongUsClient.Instance.AmHost)
                {
                    MeetingRoomManager.Instance.reporter = PlayerControl.LocalPlayer;
                    MeetingRoomManager.Instance.target = null;
                    AmongUsClient.Instance.DisconnectHandlers.AddUnique(
                    MeetingRoomManager.Instance.Cast<IDisconnectHandler>());
                    if (GameManager.Instance.CheckTaskCompletion()) return false;
                    HudManager.Instance.OpenMeetingRoom(PlayerControl.LocalPlayer);
                    PlayerControl.LocalPlayer.RpcStartMeeting(null);
                }
                return false;
            }

            return true;
        }
    }
}