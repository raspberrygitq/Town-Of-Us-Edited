using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Roles.Modifiers;

namespace TownOfUsEdited.Modifiers.VengefulMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Vengeful)) return true;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Glitch)) return true;

            var role = Modifier.GetModifier<Vengeful>(PlayerControl.LocalPlayer);
            if (__instance != role.KillButton) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (role.UsedAbility) return false;
            if (!__instance.enabled) return false;
            if (role.ClosestPlayer == null) return false;
            var taskinfos = PlayerControl.LocalPlayer.Data.Tasks.ToArray().ToList();
            var tasksLeft = taskinfos.Count(x => !x.Complete);
            if (tasksLeft > 0) return false;

            role.UsedAbility = true;

            Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);

            return false;
        }
    }
}