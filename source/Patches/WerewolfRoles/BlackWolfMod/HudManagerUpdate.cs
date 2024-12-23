using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.WerewolfRoles.BlackWolfMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite RampageSprite => TownOfUs.RampageSprite;
        public static Sprite UnRampageSprite => TownOfUs.UnRampageSprite;
        private static Sprite WerewolfConvertSprite => TownOfUs.WerewolfConvertSprite;
        
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            UpdateRampageButton(__instance);
            UpdateConvertButton(__instance);
        }
        public static void UpdateRampageButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.BlackWolf)) return;
            var role = Role.GetRole<BlackWolf>(PlayerControl.LocalPlayer);

            if (role.RampageButton == null)
            {
                role.RampageButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.RampageButton.graphic.enabled = true;
                role.RampageButton.gameObject.SetActive(false);
            }

            role.RampageButton.graphic.sprite = RampageSprite;
            role.RampageButton.transform.localPosition = new Vector3(-2f, 0f, 0f);

            if (role.RampageButton.graphic.sprite != RampageSprite &&
                role.RampageButton.graphic.sprite != UnRampageSprite)
                role.RampageButton.graphic.sprite = RampageSprite;

            if (role.RampageButton.graphic.sprite == UnRampageSprite && role.Rampaged == false)
                role.RampageButton.graphic.sprite = RampageSprite;

            if (role.Rampaged == true)
            {
                role.RampageButton.graphic.sprite = UnRampageSprite;
            }

            role.RampageButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (role.Rampaged != true)
            {
                role.RampageButton.SetCoolDown(role.RampageTimer(), CustomGameOptions.RampageCD);
                __instance.KillButton.Hide();
                __instance.ImpostorVentButton.transform.localPosition = new Vector3(0f, 1f, 0f);
            }
            else if (role.Rampaged)
            {
                __instance.ImpostorVentButton.transform.localPosition = new Vector3(-1f, 1f, 0f);
                if ((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
                {
                     __instance.KillButton.Show();
                }
            }
            if (role.RampageButton.graphic.sprite == RampageSprite)
            {

                var renderer = role.RampageButton.graphic;
                if (role.Rampaged == true || !role.RampageButton.isCoolingDown)
                {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                }
                else
                {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
                }
            }
            if (role.Rampaged == true && MeetingHud.Instance)
            {
                PlayerControl.LocalPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                Utils.Rpc(CustomRPC.UnTransform, PlayerControl.LocalPlayer.PlayerId);
                role.Rampaged = false;
            }
        }

        public static void UpdateConvertButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.BlackWolf)) return;
            var role = Role.GetRole<BlackWolf>(PlayerControl.LocalPlayer);

            if (role.ConvertButton == null)
            {
                role.ConvertButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ConvertButton.graphic.enabled = true;
                role.ConvertButton.gameObject.SetActive(false);
            }

            role.ConvertButton.graphic.sprite = WerewolfConvertSprite;
            role.ConvertButton.transform.localPosition = new Vector3(-2f, 1f, 0f);

            role.ConvertButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started
                    && role.UsedConvert != true);

            // Set the closest player for the Kill Button's targeting
            var notimps = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !x.Is(Faction.Impostors))
                .ToList();

            if (!role.UsedConvert)
            {
            Utils.SetTarget(ref role.ClosestPlayer, role.ConvertButton, float.NaN, notimps);
            }
        }
    }
}