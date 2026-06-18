using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes;
using System.Reflection;
using TownOfUsEdited.Utilities;

namespace TownOfUsEdited.Patches;

[HarmonyPatch(typeof(MedScanMinigame))]
public static class MedScanMinigameFixedUpdatePatch
{
    [HarmonyPatch(typeof(MedScanMinigame), nameof(MedScanMinigame.FixedUpdate))]
    [HarmonyPrefix]
    public static void Prefix(MedScanMinigame __instance)
    {
        if (CustomGameOptions.ParallelMedScans)
        {
            //Allows multiple medbay scans at once
            __instance.medscan.CurrentUser = PlayerControl.LocalPlayer.PlayerId;
            __instance.medscan.UsersList.Clear();
        }
    }

    [HarmonyPatch(nameof(MedScanMinigame.Begin))]
    [HarmonyPostfix]
    public static void BeginPostfix(MedScanMinigame __instance)
    {
        // Update medical details for Giant & Mini modifier
        if (PlayerControl.LocalPlayer.Is(ModifierEnum.Giant))
        {
            __instance.completeString = __instance.completeString.Replace("3' 6\"", "5' 3\"").Replace("92lb", "184lb");
        }
        else if (PlayerControl.LocalPlayer.Is(ModifierEnum.Mini))
        {
            __instance.completeString = __instance.completeString.Replace("3' 6\"", "1' 9\"").Replace("92lb", "46lb");
        }
    }

    [HarmonyPatch]
    public static class MedscanWalkPadPatch
    {
        public static MethodBase TargetMethod()
        {
            return StateMachineWrapper<MedScanMinigame>.GetStateMachineMoveNext(nameof(MedScanMinigame.WalkToPad))!;
        }

        public static bool Prefix(Il2CppObjectBase __instance)
        {
            if (CustomGameOptions.MedscanWalk) return true;

            var wrapper = new StateMachineWrapper<MedScanMinigame>(__instance);
            MedScanMinigame medScanMinigame = wrapper.Instance;
            var num = medScanMinigame.state;
            var negative = -1;
            switch (num)
            {
                case MedScanMinigame.PositionState.None:
                    medScanMinigame.state = MedScanMinigame.PositionState.WalkingToPad;
                    break;
                case MedScanMinigame.PositionState.WalkingToPad:
                    medScanMinigame.state = MedScanMinigame.PositionState.WalkingToOffset;
                    break;
                case MedScanMinigame.PositionState.WalkingToOffset:
                    medScanMinigame.state = (MedScanMinigame.PositionState)negative;
                    medScanMinigame.walking = null;
                    break;
            }
            return false;
        }
    }
    [HarmonyPatch]
    public static class MedscanWalkOffsetPatch
    {
        public static MethodBase TargetMethod()
        {
            return StateMachineWrapper<MedScanMinigame>.GetStateMachineMoveNext(nameof(MedScanMinigame.WalkToOffset))!;
        }

        public static bool Prefix(Il2CppObjectBase __instance)
        {
            if (CustomGameOptions.MedscanWalk) return true;

            var wrapper = new StateMachineWrapper<MedScanMinigame>(__instance);
            MedScanMinigame medScanMinigame = wrapper.Instance;
            var num = medScanMinigame.state;
            var negative = -1;
            switch (num)
            {
                case MedScanMinigame.PositionState.None:
                    medScanMinigame.state = MedScanMinigame.PositionState.WalkingToPad;
                    break;
                case MedScanMinigame.PositionState.WalkingToPad:
                    medScanMinigame.state = MedScanMinigame.PositionState.WalkingToOffset;
                    break;
                case MedScanMinigame.PositionState.WalkingToOffset:
                    medScanMinigame.state = (MedScanMinigame.PositionState)negative;
                    medScanMinigame.walking = null;
                    break;
            }
            return false;
        }
    }
}