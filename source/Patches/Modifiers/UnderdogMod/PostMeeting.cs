using HarmonyLib;
using TownOfUsEdited.Roles.Modifiers;

namespace TownOfUsEdited.Modifiers.UnderdogMod
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static class HUDClose
    {
        public static void Postfix()
        {
            if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Underdog)) return;
            var modifier = Modifier.GetModifier<Underdog>(PlayerControl.LocalPlayer);
            modifier.SetKillTimer();
        }
    }
}
