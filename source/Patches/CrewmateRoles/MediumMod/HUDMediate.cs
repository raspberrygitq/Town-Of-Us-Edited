using HarmonyLib;
using System;
using System.Linq;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CrewmateRoles.MediumMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HUDMediate
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Medium))
            {
                var mediateButton = __instance.KillButton;
                var mediateText = __instance.KillButton.buttonLabelText;

                var role = Role.GetRole<Medium>(PlayerControl.LocalPlayer);
                mediateButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
                mediateText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

                mediateText.text = "Mediate";
                mediateText.SetOutlineColor(Patches.Colors.Medium);

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (role.MediatedPlayers.Keys.Contains(player.PlayerId))
                    {
                        role.MediatedPlayers.GetValueSafe(player.PlayerId).target = player.transform.position;
                        player.Visible = true;
                        if (!CustomGameOptions.ShowMediatePlayer)
                        {
                            player.SetOutfit(CustomPlayerOutfitType.Camouflage, new NetworkedPlayerInfo.PlayerOutfit()
                            {
                                ColorId = player.GetDefaultOutfit().ColorId,
                                HatId = "",
                                SkinId = "",
                                VisorId = "",
                                PlayerName = " ",
                                PetId = ""
                            });
                            PlayerMaterial.SetColors(Color.grey, player.myRend());
                        }
                    }
                }
                mediateButton.SetCoolDown(role.MediateTimer(), CustomGameOptions.MediateCooldown);

                var renderer = mediateButton.graphic;
                if (!mediateButton.isCoolingDown && PlayerControl.LocalPlayer.moveable)
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                    mediateText.color = Palette.EnabledColor;
                    mediateText.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    renderer.color = Palette.DisabledClear;
                    renderer.material.SetFloat("_Desat", 1f);
                    mediateText.color = Palette.DisabledClear;
                    mediateText.material.SetFloat("_Desat", 1f);
                }
            }
            if (CustomGameOptions.ShowMediumToDead)
            {
                foreach (var medRole in Role.GetRoles(RoleEnum.Medium))
                {
                    var medium = (Medium)medRole;
                    if (medium.MediatedPlayers.ContainsKey(PlayerControl.LocalPlayer.PlayerId))
                    {
                        medium.MediatedPlayers.GetValueSafe(PlayerControl.LocalPlayer.PlayerId).target = medium.Player.transform.localPosition;
                    }
                }
            }
        }
    }
}