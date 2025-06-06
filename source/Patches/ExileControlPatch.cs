using HarmonyLib;
using TownOfUsEdited.Patches.CovenRoles;
using TownOfUsEdited.Patches.CrewmateRoles.JailorMod;
using TownOfUsEdited.Patches.ImpostorRoles;
using TownOfUsEdited.Patches.Modifiers.LoversMod;
using TownOfUsEdited.Patches.NeutralRoles.SerialKillerMod;
using TownOfUsEdited.Patches.NeutralRoles.VampireMod;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
    [HarmonyPriority(Priority.First)]
    class ExileControllerPatch
    {
        public static ExileController lastExiled;
        public static void Prefix(ExileController __instance)
        {
            lastExiled = __instance;
            ImpostorChat.UpdateImpostorChat();
            CovenChat.UpdateCovenChat();
            VampireChat.UpdateVampireChat();
            SerialKillerChat.UpdateSerialKillerChat();
            LoversChat.UpdateLoversChat();
            JailorChat.UpdateJailorChat();
        }
    }
}