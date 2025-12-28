using HarmonyLib;
using UnityEngine;
using UObject = UnityEngine.Object;
using System.Collections.Generic;

namespace TownOfUsEdited.Patches.BetterMaps;

[HarmonyPatch(typeof(ShipStatus))]

public static class BetterAirship
{
    public static bool GameStarted;
    public static readonly List<byte> SpawnPoints = new();

    [HarmonyPatch(typeof(AirshipStatus), nameof(AirshipStatus.OnEnable))]
    public static class Repositioning
    {
        public static void Postfix()
        {
            if (CustomGameOptions.BAMoveAdmin != 0)
            {
                var adminTable = UObject.FindObjectOfType<MapConsole>();
                var mapFloating = GameObject.Find("Cockpit/cockpit_mapfloating");

                if ((int)CustomGameOptions.BAMoveAdmin == 1)
                {
                    adminTable.transform.position = new(-17.269f, 1.375f);
                    adminTable.transform.rotation = Quaternion.Euler(new(0, 0, 350.316f));
                    adminTable.transform.localScale = new(1, 1, 1);

                    mapFloating.transform.position = new(-17.736f, 2.36f);
                    mapFloating.transform.rotation = Quaternion.Euler(new(0, 0, 350));
                    mapFloating.transform.localScale = new(1, 1, 1);
                }
                else if ((int)CustomGameOptions.BAMoveAdmin == 2)
                {
                    //New Admin
                    adminTable.transform.position = new(5.078f, 3.4f, 1);
                    adminTable.transform.rotation = Quaternion.Euler(new(0, 0, 76.1f));
                    adminTable.transform.localScale = new(1.200f, 1.700f, 1);
                    mapFloating.transform.localScale = new(0, 0, 0);
                }
            }

            if (CustomGameOptions.BAMoveElectrical != 0)
            {
                var electrical = GameObject.Find("GapRoom/task_lightssabotage (gap)");

                if ((int)CustomGameOptions.BAMoveElectrical == 1)
                {
                    electrical.transform.position = new(-8.817f, 13.184f);
                    electrical.transform.localScale = new(0.909f, 0.818f, 1);

                    var originalSupport = GameObject.Find("Vault/cockpit_comms");
                    var supportElectrical = UObject.Instantiate(originalSupport, originalSupport.transform);

                    supportElectrical.transform.position = new(-8.792f, 13.242f);
                    supportElectrical.transform.localScale = new(1, 1, 1);
                }
                else if ((int)CustomGameOptions.BAMoveElectrical == 2)
                    electrical.transform.position = new(19.339f, -3.665f);
            }

            if (CustomGameOptions.BAMoveVitals)
            {
                GameObject.Find("Medbay/panel_vitals").transform.position = new(24.55f, -4.780f);
                GameObject.Find("Medbay/panel_data").transform.position = new(25.240f, -7.938f);
            }

            if (CustomGameOptions.BAMoveFuel)
                GameObject.Find("Storage/task_gas").transform.position = new(36.070f, 1.897f);

            if (CustomGameOptions.BAMoveDivert)
                GameObject.Find("HallwayMain/DivertRecieve").transform.position = new(13.35f, -1.659f);
        }
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    public static class GameEndedPatch
    {
        public static void Prefix() => GameStarted = false;
    }
}