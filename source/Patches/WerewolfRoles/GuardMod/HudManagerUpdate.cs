using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.WerewolfRoles.GuardMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudManagerUpdate
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Guard)) return;
            var ProtectButton = __instance.KillButton;

            var role = Role.GetRole<Guard>(PlayerControl.LocalPlayer);

            ProtectButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started
                    && role.UsedProtect != true);
            
            if (!role.UsedProtect)
            {
            Utils.SetTarget(ref role.ClosestPlayer, ProtectButton, float.NaN);
            }
        }
    }
}