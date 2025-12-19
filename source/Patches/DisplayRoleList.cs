using AmongUs.GameOptions;
using HarmonyLib;
using System.Collections.Generic;
using TMPro;
using TownOfUsEdited.CustomOption;
using UnityEngine;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    internal static class DisplayRoleList
    {
        public static TextMeshPro RoleList;

        private static readonly List<string> roleListText = new List<string>
        {
            "<color=#66FFFFFF>Crew</color> Investigative",
            "<color=#66FFFFFF>Crew</color> Killing",
            "<color=#66FFFFFF>Crew</color> Power",
            "<color=#66FFFFFF>Crew</color> Protective",
            "<color=#66FFFFFF>Crew</color> Support",
            "Common <color=#66FFFFFF>Crew</color>",
            "Special <color=#66FFFFFF>Crew</color>",
            "Random <color=#66FFFFFF>Crew</color>",
            "<color=#999999FF>Neutral</color> Benign",
            "<color=#999999FF>Neutral</color> Evil",
            "<color=#999999FF>Neutral</color> Killing",
            "Common <color=#999999FF>Neutral</color>",
            "Random <color=#999999FF>Neutral</color>",
            "<color=#FF0000FF>Imp</color> Concealing",
            "<color=#FF0000FF>Imp</color> Killing",
            "<color=#FF0000FF>Imp</color> Support",
            "Common <color=#FF0000FF>Imp</color>",
            "Random <color=#FF0000FF>Imp</color>",
            "<color=#bf5fff>Coven</color> Killing",
            "<color=#bf5fff>Coven</color> Support",
            "Common <color=#bf5fff>Coven</color>",
            "Random <color=#bf5fff>Coven</color>",
            "Non-<color=#FF0000FF>Imp</color> / <color=#bf5fff>Coven</color>",
            "Any"
        };

        private static string GetRoleForSlot(int slotValue)
        {
            if (slotValue >= 0 && slotValue < roleListText.Count)
            {
                return roleListText[slotValue]; 
            }
            else
            {
                return "<color=#696969>Unknown</color>"; 
            }
        }

        public static void Postfix(HudManager __instance)
        {
            if (AmongUsClient.Instance?.GameState != InnerNet.InnerNetClient.GameStates.Joined) return;
            if (AmongUsClient.Instance?.NetworkMode == NetworkModes.FreePlay) return;
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return;
            if (CustomGameOptions.GameMode != GameMode.RoleList)
            {
                if (RoleList != null)
                {
                    UnityEngine.Object.Destroy(RoleList.gameObject);
                    RoleList = null;
                }
                return;
            }

            var pingTracker = UnityEngine.Object.FindObjectOfType<PingTracker>(true);
            if (pingTracker == null)
                return;

            if (RoleList != null)
            {
                UnityEngine.Object.Destroy(RoleList.gameObject);
                RoleList = null;
            }

            RoleList = UnityEngine.Object.Instantiate(pingTracker.GetComponent<TextMeshPro>(), __instance.transform);
            if (RoleList != null)
            {
                string rolelist = string.Empty;

                var players = GameData.Instance.PlayerCount;
                int maxSlots = players < 15 ? players : 15; 

                rolelist = string.Empty;
                for (int i = 0; i < maxSlots; i++)
                {
                    int slotValue = i switch
                    {
                        0 => Generate.Slot1.Get(), 
                        1 => Generate.Slot2.Get(), 
                        2 => Generate.Slot3.Get(), 
                        3 => Generate.Slot4.Get(), 
                        4 => Generate.Slot5.Get(), 
                        5 => Generate.Slot6.Get(), 
                        6 => Generate.Slot7.Get(), 
                        7 => Generate.Slot8.Get(), 
                        8 => Generate.Slot9.Get(), 
                        9 => Generate.Slot10.Get(), 
                        10 => Generate.Slot11.Get(), 
                        11 => Generate.Slot12.Get(), 
                        12 => Generate.Slot13.Get(), 
                        13 => Generate.Slot14.Get(),
                        14 => Generate.Slot15.Get(),
                        _ => -1 
                    };

                    rolelist += $"{GetRoleForSlot(slotValue)}\n";  
                }

                RoleList.alignment = TextAlignmentOptions.TopLeft;
                RoleList.verticalAlignment = VerticalAlignmentOptions.Top;
                RoleList.transform.localPosition = new Vector3(-4.9f, 2.9f, 0);
                RoleList.fontSize = RoleList.fontSizeMin = RoleList.fontSizeMax = 3f;

                RoleList.text = $"<color=#FFD700>Role List:</color>\n{rolelist}";
                RoleList.enabled = true;
            }
        }
    }

    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
    internal static class HideRoleListInIntroCutscene
    {
        private static void Postfix()
        {
            if (DisplayRoleList.RoleList != null)
            {
                DisplayRoleList.RoleList.enabled = false;
            }
        }
    }

    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    internal static class RevealRoleListInLobby
    {
        private static void Postfix()
        {
            if (DisplayRoleList.RoleList != null)
            {
                DisplayRoleList.RoleList.enabled = true;
            }
        }
    }
}