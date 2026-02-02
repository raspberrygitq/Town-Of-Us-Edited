using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.NeutralRoles.SerialKillerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            var player = PlayerControl.LocalPlayer;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller)) return;

            // Get the Serial Killer role instance
            var sk = Role.GetRole<SerialKiller>(PlayerControl.LocalPlayer);

            // Check if the game state allows the KillButton to be active
            bool isKillButtonActive = __instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled;
            isKillButtonActive = isKillButtonActive && !MeetingHud.Instance && !player.Data.IsDead;
            isKillButtonActive = isKillButtonActive && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
            AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay);

            // Set KillButton's visibility
            __instance.KillButton.gameObject.SetActive(isKillButtonActive);

            if (sk.skconvertButton == null)
            {
                sk.skconvertButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                sk.skconvertButton.graphic.enabled = true;
                sk.skconvertButton.gameObject.SetActive(false);
            }

            sk.skconvertButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay)
                    && sk.Converted == false && CustomGameOptions.SerialKillerCanConvert
                    && !player.Data.Disconnected);
            sk.skconvertButton.buttonLabelText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay)
                    && sk.Converted == false && CustomGameOptions.SerialKillerCanConvert
                    && !player.Data.Disconnected);

            sk.skconvertButton.graphic.sprite = TownOfUsEdited.SKConvertSprite;
            sk.skconvertButton.transform.localPosition = new Vector3(-1f, 1f, 0f);

            sk.skconvertButton.buttonLabelText.text = "Convert";
            sk.skconvertButton.buttonLabelText.SetOutlineColor(Patches.Colors.SerialKiller);

            // Set KillButton's cooldown
            __instance.KillButton.SetCoolDown(sk.KillTimer(), CustomGameOptions.SerialKillerKCD);

            // Set the closest player for the Kill Button's targeting
            var notsk = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !x.Is(RoleEnum.SerialKiller))
                .ToList();

            var notskorlover = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !x.Is(RoleEnum.SerialKiller) && !x.IsLover())
                .ToList();

            var sks = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.SerialKiller)).ToList();

            Utils.SetTarget(ref sk.ClosestPlayer, sk.skconvertButton, float.NaN, notsk);

            if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref sk.ClosestPlayer, __instance.KillButton);
            else if (PlayerControl.LocalPlayer.IsLover() && CustomGameOptions.ImpLoverKillTeammate) Utils.SetTarget(ref sk.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover()).ToList());
            else if (PlayerControl.LocalPlayer.IsLover()) Utils.SetTarget(ref sk.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(RoleEnum.SerialKiller)).ToList());
            else Utils.SetTarget(ref sk.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(RoleEnum.SerialKiller)).ToList());

            sk.skconvertButton.SetCoolDown(sk.SKTimer(), CustomGameOptions.SerialKillerKCD);
            sk.skconvertButton.graphic.SetCooldownNormalizedUvs();
        }
    }
}