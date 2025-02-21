using System.Linq;
using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CrewmateRoles.KnightMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            var player = PlayerControl.LocalPlayer;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Knight)) return;

            // Get the Knight role instance
            var knight = Role.GetRole<Knight>(PlayerControl.LocalPlayer);

            if (knight.UsesText == null && knight.UsesLeft > 0)
            {
                knight.UsesText = Object.Instantiate(__instance.KillButton.cooldownTimerText, __instance.KillButton.transform);
                knight.UsesText.gameObject.SetActive(false);
                knight.UsesText.transform.localPosition = new Vector3(
                    knight.UsesText.transform.localPosition.x + 0.26f,
                    knight.UsesText.transform.localPosition.y + 0.29f,
                    knight.UsesText.transform.localPosition.z);
                knight.UsesText.transform.localScale = knight.UsesText.transform.localScale * 0.65f;
                knight.UsesText.alignment = TMPro.TextAlignmentOptions.Right;
                knight.UsesText.fontStyle = TMPro.FontStyles.Bold;
            }
            if (knight.UsesText != null)
            {
                knight.UsesText.text = knight.UsesLeft + "";
            }

            // Check if the game state allows the KillButton to be active
            bool isKillButtonActive = __instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled;
            isKillButtonActive = isKillButtonActive && !MeetingHud.Instance && !player.Data.IsDead;
            isKillButtonActive = isKillButtonActive && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started;

            // Set KillButton's visibility
            __instance.KillButton.gameObject.SetActive(isKillButtonActive);

            //Set Uses Text's visibility
             knight.UsesText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            // Set KillButton's cooldown
            if (knight.ButtonUsable)
            {
                __instance.KillButton.SetCoolDown(knight.KillTimer(), CustomGameOptions.KnightKCD);
                if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref knight.ClosestPlayer, __instance.KillButton);
                else if (knight.Player.IsLover() && knight.Player.Is(Faction.Madmates) && (!CustomGameOptions.MadmateKillEachOther || CustomGameOptions.GameMode == GameMode.Cultist)) Utils.SetTarget(ref knight.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Impostors)).ToList());
                else if (knight.Player.Is(Faction.Madmates) && (!CustomGameOptions.MadmateKillEachOther || CustomGameOptions.GameMode == GameMode.Cultist)) Utils.SetTarget(ref knight.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Impostors)).ToList());
                else if (knight.Player.IsLover()) Utils.SetTarget(ref knight.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover()).ToList());
                else Utils.SetTarget(ref knight.ClosestPlayer, __instance.KillButton);
            }
            else
            {
                __instance.KillButton.SetCoolDown(0f, CustomGameOptions.KnightKCD);
            }

            return;
            
        }
    }
}