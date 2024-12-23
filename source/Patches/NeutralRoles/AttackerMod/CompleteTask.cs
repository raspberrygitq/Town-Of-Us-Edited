using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.AttackerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Attacker)) return;
            if (__instance.Data.IsDead) return;
            var taskinfos = __instance.Data.Tasks.ToArray();

            var tasksLeft = taskinfos.Count(x => !x.Complete);
            var role = Role.GetRole<Attacker>(__instance);
            if (tasksLeft == 0)
            {
                role.TurnTerrorist();
                Utils.Rpc(CustomRPC.TurnTerrorist, PlayerControl.LocalPlayer.PlayerId);
            }
        }
    }
}