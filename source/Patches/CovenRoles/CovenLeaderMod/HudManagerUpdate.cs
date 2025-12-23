using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Patches;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CovenRoles.CovenLeaderMod
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
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.CovenLeader)) return;

            var role = Role.GetRole<CovenLeader>(PlayerControl.LocalPlayer);

            if (role.RecruitButton == null)
            {
                role.RecruitButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.RecruitButton.graphic.enabled = true;
                role.RecruitButton.gameObject.SetActive(false);
            }

            role.RecruitButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay)
                    && !role.Converted);

            role.RecruitButton.buttonLabelText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay)
                    && !role.Converted);

            role.RecruitButton.buttonLabelText.text = "Recruit";
            role.RecruitButton.buttonLabelText.SetOutlineColor(Colors.Coven);

            role.RecruitButton.graphic.sprite = TownOfUsEdited.Recruit;
            role.RecruitButton.transform.localPosition = new Vector3(-2f, 1f, 0f);

            role.RecruitButton.SetCoolDown(role.KillCooldown, CustomGameOptions.CovenKCD);
            role.RecruitButton.graphic.SetCooldownNormalizedUvs();

            // Set the closest player for the Kill Button's targeting
            var notcoven = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !x.Is(Faction.Coven) && !x.Data.IsDead)
                .ToList();

            Utils.SetTarget(ref role.ClosestPlayer, role.RecruitButton, float.NaN, notcoven);

            var labelrender = role.RecruitButton.buttonLabelText;
            if (role.ClosestPlayer != null)
            {
                labelrender.color = Palette.EnabledColor;
                labelrender.material.SetFloat("_Desat", 0f);
            }
            else
            {
                labelrender.color = Palette.DisabledClear;
                labelrender.material.SetFloat("_Desat", 1f);
            }
        }
    }
}