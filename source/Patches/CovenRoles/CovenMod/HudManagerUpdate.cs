using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CovenRoles.CovenMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            var player = PlayerControl.LocalPlayer;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Coven)) return;

            var role = Role.GetRole<Coven>(PlayerControl.LocalPlayer);

            // Check if the game state allows the KillButton to be active
            bool isKillButtonActive = __instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled;
            isKillButtonActive = isKillButtonActive && !MeetingHud.Instance && !player.Data.IsDead;
            isKillButtonActive = isKillButtonActive && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started;

            // Set KillButton's visibility
            __instance.KillButton.gameObject.SetActive(isKillButtonActive);

            if (role.SabotageButton == null)
            {
                role.SabotageButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.SabotageButton.graphic.enabled = true;
                role.SabotageButton.gameObject.SetActive(false);
            }

            role.SabotageButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
                    
            role.SabotageButton.graphic.sprite = TownOfUs.SabotageCoven;
            if (!PlayerControl.LocalPlayer.Data.IsDead)
            {
                role.SabotageButton.transform.localPosition = new Vector3(-1f, 1f, 0f);
            }
            else
            {
                var position = __instance.KillButton.transform.localPosition;
                role.SabotageButton.transform.localPosition = new Vector3(position.x,
                position.y, position.z);
            }

            // Set KillButton's cooldown
            __instance.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.CovenKCD);
            role.SabotageButton.SetCoolDown(0f, 1f);

            // Set the closest player for the Kill Button's targeting
            var notcoven = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !x.Is(Faction.Coven) && !x.Data.IsDead)
                .ToList();

            if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
            else if (PlayerControl.LocalPlayer.IsLover() && CustomGameOptions.ImpLoverKillTeammate) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover()).ToList());
            else if (PlayerControl.LocalPlayer.IsLover()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Coven)).ToList());
            else Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Coven)).ToList());
            
            role.SabotageButton.graphic.color = Palette.EnabledColor;
            role.SabotageButton.graphic.material.SetFloat("_Desat", 0f);
        }
    }

    [HarmonyPatch(typeof(NormalGameManager), nameof(NormalGameManager.GetMapOptions))]
    public class MapPatch
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(ref MapOptions __result)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(Faction.Coven)) return;
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            if (MeetingHud.Instance) return;
            __result = new MapOptions
		    {
			    Mode = MapOptions.Modes.Sabotage
		    };
        }
    }

    //Code by 50, made me win more time ty
    [HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageReactor))]
    public class PreventReactor
    {
        public static bool Prefix()
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return true;
            if (PlayerControl.LocalPlayer == null) return true;
            if (PlayerControl.LocalPlayer.Data == null) return true;
            if (!PlayerControl.LocalPlayer.Is(Faction.Coven)) return true;
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return true;
            return false;
        }
    }
    [HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageOxygen))]
    public class PreventOxygen
    {
        public static bool Prefix()
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return true;
            if (PlayerControl.LocalPlayer == null) return true;
            if (PlayerControl.LocalPlayer.Data == null) return true;
            if (!PlayerControl.LocalPlayer.Is(Faction.Coven)) return true;
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return true;
            return false;
        }
    }
    [HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageSeismic))]
    public class PreventSeismic
    {
        public static bool Prefix()
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return true;
            if (PlayerControl.LocalPlayer == null) return true;
            if (PlayerControl.LocalPlayer.Data == null) return true;
            if (!PlayerControl.LocalPlayer.Is(Faction.Coven)) return true;
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return true;
            return false;
        }
    }
    [HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageHeli))]
    public class PreventHeli
    {
        public static bool Prefix()
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return true;
            if (PlayerControl.LocalPlayer == null) return true;
            if (PlayerControl.LocalPlayer.Data == null) return true;
            if (!PlayerControl.LocalPlayer.Is(Faction.Coven)) return true;
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return true;
            return false;
        }
    }
}