using HarmonyLib;
using InnerNet;
using System.Collections;
using System.Linq;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CrewmateRoles.JailorMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class UpdateJailButton
    {
        public static IEnumerator Jail(PlayerControl jailedPlayer)
        {
            GameObject[] lockImg = { null, null };

            while (true)
            {
                if (PlayerControl.LocalPlayer == jailedPlayer)
                {
                    if (HudManager.Instance.KillButton != null && HudManager.Instance.KillButton.isActiveAndEnabled)
                    {
                        if (lockImg[0] == null)
                        {
                            lockImg[0] = new GameObject();
                            var lockImgR = lockImg[0].AddComponent<SpriteRenderer>();
                            lockImgR.sprite = TownOfUsEdited.LockSprite;
                        }
                        lockImg[0].layer = 5;
                        lockImg[0].transform.position = new Vector3(HudManager.Instance.KillButton.transform.position.x, HudManager.Instance.KillButton.transform.position.y, -50f);
                    }

                    var role = Role.GetRole(PlayerControl.LocalPlayer);
                    if (role?.ExtraButtons.Count > 0)
                    {
                        if (lockImg[1] == null)
                        {
                            lockImg[1] = new GameObject();
                            var lockImgR = lockImg[1].AddComponent<SpriteRenderer>();
                            lockImgR.sprite = TownOfUsEdited.LockSprite;
                        }

                        lockImg[1].transform.position = new Vector3(
                            role.ExtraButtons[0].transform.position.x,
                            role.ExtraButtons[0].transform.position.y, -50f);
                        lockImg[1].layer = 5;
                    }
                }

                if (MeetingHud.Instance || jailedPlayer?.Data.IsDead == true || (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started &&
                AmongUsClient.Instance.NetworkMode != NetworkModes.FreePlay) || !jailedPlayer.IsJailed())
                {
                    foreach (var obj in lockImg)
                    {
                        obj?.SetActive(false);
                    }
                    yield break;
                }

                yield return null;
            }
        }

        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jailor)) return;
            var jailButton = __instance.KillButton;

            var role = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
            var jailText = __instance.KillButton.buttonLabelText;

            if (role.ReleaseButton == null)
            {
                role.ReleaseButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ReleaseButton.graphic.enabled = true;
                role.ReleaseButton.gameObject.SetActive(false);
                role.ReleaseText = Object.Instantiate(__instance.KillButton.buttonLabelText, role.ReleaseButton.transform);
                role.ReleaseText.gameObject.SetActive(false);
                role.ButtonLabels.Add(role.ReleaseText);
            }

            role.ReleaseButton.graphic.sprite = TownOfUsEdited.ReleaseSprite;
            role.ReleaseButton.transform.localPosition = new Vector3(-2f, 0f, 0f);

            role.ReleaseButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.ReleaseText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.ReleaseText.text = "Release";
            role.ReleaseText.SetOutlineColor(Patches.Colors.Jailor);

            jailButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            jailText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            jailButton.SetCoolDown(role.JailTimer(), CustomGameOptions.JailCD);

            var notJailed = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => x != role.JailedPlayer)
                .ToList();

            if (role.JailedPlayer == null)
            {
                if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, notJailed);
                else if (role.Player.IsLover() && role.Player.Is(Faction.Madmates) && (!CustomGameOptions.MadmateKillEachOther || CustomGameOptions.GameMode == GameMode.Cultist)) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Impostors) && x != role.JailedPlayer).ToList());
                else if (role.Player.Is(Faction.Madmates) && (!CustomGameOptions.MadmateKillEachOther || CustomGameOptions.GameMode == GameMode.Cultist)) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Impostors) && x != role.JailedPlayer).ToList());
                else if (role.Player.IsLover()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && x != role.JailedPlayer).ToList());
                else Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, notJailed);
            }
            else
            {
                jailButton.SetTarget(null);
                role.ClosestPlayer = null;
            }

            if (role.JailedPlayer == null && role.ClosestPlayer != null && !jailButton.isCoolingDown)
            {
                jailButton.graphic.color = Palette.EnabledColor;
                jailButton.graphic.material.SetFloat("_Desat", 0f);
                jailText.color = Palette.EnabledColor;
                jailText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                jailButton.graphic.color = Palette.DisabledClear;
                jailButton.graphic.material.SetFloat("_Desat", 1f);
                jailText.color = Palette.DisabledClear;
                jailText.material.SetFloat("_Desat", 1f);
            }

            if (role.JailedPlayer != null)
            {
                role.ReleaseButton.graphic.color = Palette.EnabledColor;
                role.ReleaseButton.graphic.material.SetFloat("_Desat", 0f);
                role.ReleaseText.color = Palette.EnabledColor;
                role.ReleaseText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                role.ReleaseButton.graphic.color = Palette.DisabledClear;
                role.ReleaseButton.graphic.material.SetFloat("_Desat", 1f);
                role.ReleaseText.color = Palette.DisabledClear;
                role.ReleaseText.material.SetFloat("_Desat", 1f);
            }
        }
    }
}