using HarmonyLib;
using UnityEngine;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Begin))]
    public class NoVitals
    {
        public static bool Prefix(VitalsMinigame __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return true;
            if ((PlayerControl.LocalPlayer.Is(RoleEnum.Transporter) && !CustomGameOptions.TransporterVitals) ||
                (PlayerControl.LocalPlayer.Is(RoleEnum.TimeLord) && !CustomGameOptions.TimeLordVitals) ||
                (PlayerControl.LocalPlayer.Is(RoleEnum.Lookout) && !CustomGameOptions.LookoutUseVitals))
            {
                Object.Destroy(__instance.gameObject);
                return false;
            }

            return true;
        }
    }
}