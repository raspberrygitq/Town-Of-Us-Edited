using HarmonyLib;
using System.Reflection;
using TownOfUsEdited.Utilities;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch]
    public class KillAnimationPatches
    {
        public static MethodBase TargetMethod()
        {
            return StateMachineWrapper<OverlayKillAnimation>.GetStateMachineMoveNext(nameof(OverlayKillAnimation.CoShow))!;
        }
        public static void Postfix(bool __result)
        {
            if (MeetingHud.Instance)
            {
                foreach (var state in MeetingHud.Instance.playerStates)
                {
                    state.gameObject.SetActive(!__result);
                }
            }
        }
    }
}