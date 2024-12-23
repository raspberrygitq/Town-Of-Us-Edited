using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.LighterMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Lighter);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Lighter>(PlayerControl.LocalPlayer);
            var lightButton = DestroyableSingleton<HudManager>.Instance.KillButton;
            if (__instance == lightButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.Cooldown > 0) return false;
                var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                if (!abilityUsed) return false;
                role.TimeRemaining = CustomGameOptions.LightDuration;
                role.StartLight();
                return false;
            }

            return true;
        }
    }
}