using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Roles.Modifiers;

namespace TownOfUsEdited.ImpostorRoles.GrenadierMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite FlashSprite => TownOfUsEdited.FlashSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Grenadier)) return;
            var role = Role.GetRole<Grenadier>(PlayerControl.LocalPlayer);
            if (role.FlashButton == null)
            {
                role.FlashButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.FlashButton.graphic.enabled = true;
                role.FlashButton.gameObject.SetActive(false);
            }

            if (!PlayerControl.LocalPlayer.IsHypnotised())
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    var data = player?.Data;
                    if (data == null || data.Disconnected || data.IsDead || player.Data.IsImpostor())
                        continue;

                    var tempColour = player.nameText().color;
                    if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                        player.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper &&
                        role.flashedPlayers.Contains(player))
                    {
                        var colour = Color.black;
                        if (player.Is(ModifierEnum.Shy)) colour.a = Modifier.GetModifier<Shy>(player).Opacity;
                        player.nameText().color = colour;
                        player.myRend().material.SetColor("_VisorColor", Color.black);
                    }
                    else
                    {
                        player.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
                        player.nameText().color = tempColour;
                    }
                }
            }

            role.FlashButton.graphic.sprite = FlashSprite;
            role.FlashButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.FlashButton.transform.localPosition = new Vector3(-2f, 1f, 0f);

            if (role.Flashed)
            {
                role.FlashButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.GrenadeDuration);
                return;
            }

            role.FlashButton.SetCoolDown(role.FlashTimer(), CustomGameOptions.GrenadeCd);

            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            var sabActive = system.AnyActive;

            if (sabActive)
            {
                role.FlashButton.graphic.color = Palette.DisabledClear;
                role.FlashButton.graphic.material.SetFloat("_Desat", 1f);
                return;
            }

            role.FlashButton.graphic.color = Palette.EnabledColor;
            role.FlashButton.graphic.material.SetFloat("_Desat", 0f);
        }
    }
}