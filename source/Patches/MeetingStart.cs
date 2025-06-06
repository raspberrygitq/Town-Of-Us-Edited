using HarmonyLib;
using TownOfUsEdited.Extensions;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (ShowShield.FirstRoundShielded != null && !ShowShield.FirstRoundShielded.Data.Disconnected)
            {
                ShowShield.FirstRoundShielded.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
                ShowShield.FirstRoundShielded.myRend().material.SetFloat("_Outline", 0f);
                ShowShield.FirstRoundShielded = null;
            }
        }
    }
}