using HarmonyLib;

namespace TownOfUsEdited
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class CamouflageUnCamouflage
    {
        public static bool CommsEnabled;

        public static bool IsCamoed => CommsEnabled;

        public static void Postfix()
        {
            if (CustomGameOptions.ColourblindComms)
            {
                if (ShipStatus.Instance != null)
                {
                    if (Utils.CommsCamouflaged())
                    {
                        CommsEnabled = true;
                        Utils.GroupCamouflage();
                    }
                }

                if (CommsEnabled)
                {
                    CommsEnabled = false;
                    Utils.UnCamouflage();
                }
            }
        }
    }
}