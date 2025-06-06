using HarmonyLib;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch(typeof(OverlayKillAnimation._CoShow_d__18), nameof(OverlayKillAnimation._CoShow_d__18.MoveNext))]
    public class KillAnimationPatches
    {
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