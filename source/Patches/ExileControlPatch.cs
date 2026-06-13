using HarmonyLib;
using TownOfUsEdited.CrewmateRoles.JailorMod;
using TownOfUsEdited.Modifiers.LoversMod;
using TownOfUsEdited.NeutralRoles.SerialKillerMod;
using TownOfUsEdited.NeutralRoles.VampireMod;
using TownOfUsEdited.Patches.CovenRoles;
using TownOfUsEdited.Patches.ImpostorRoles;

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