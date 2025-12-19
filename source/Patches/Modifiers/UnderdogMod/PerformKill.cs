using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Roles.Modifiers;

namespace TownOfUsEdited.Modifiers.UnderdogMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public class PerformKill
    {
        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            if (!__instance.Is(ModifierEnum.Underdog)) return;
            var modifier = Modifier.GetModifier<Underdog>(__instance);
            modifier.SetKillTimer();
        }

        internal static bool LastImp()
        {
            return PlayerControl.AllPlayerControls.ToArray()
                .Count(x => x.Data.IsImpostor() && !x.Data.IsDead) == 1;
        }

        internal static bool IncreasedKC()
        {
            if (CustomGameOptions.UnderdogIncreasedKC)
                return false;
            else
                return true;
        }
    }
}
