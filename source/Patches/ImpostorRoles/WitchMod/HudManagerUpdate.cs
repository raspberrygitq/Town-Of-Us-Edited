using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;
using System.Linq;

namespace TownOfUsEdited.ImpostorRoles.WitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class PlayerControlUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Witch)) return;

            var role = Role.GetRole<Witch>(PlayerControl.LocalPlayer);
            if (role.SpellButton == null)
            {
                role.SpellButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.SpellButton.graphic.enabled = true;
                role.SpellButton.gameObject.SetActive(false);
                role.SpellText = Object.Instantiate(__instance.KillButton.buttonLabelText, role.SpellButton.transform);
                role.SpellText.gameObject.SetActive(false);
                role.ButtonLabels.Add(role.SpellText);
            }

            role.SpellButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
            role.SpellButton.graphic.sprite = TownOfUsEdited.Spell;
            role.SpellButton.transform.localPosition = new Vector3(-2f, 1f, 0f);

            role.SpellText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

            role.SpellText.text = "Spell";

            var killButton = role.SpellButton;
            if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayer, killButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsCursed()).ToList());
            else if (PlayerControl.LocalPlayer.IsLover() && CustomGameOptions.ImpLoverKillTeammate) Utils.SetTarget(ref role.ClosestPlayer, killButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.IsCursed()).ToList());
            else if (PlayerControl.LocalPlayer.IsLover() && !CustomGameOptions.MadmateKillEachOther) Utils.SetTarget(ref role.ClosestPlayer, killButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Impostors) && !x.Is(Faction.Madmates) && !x.IsCursed()).ToList());
            else if (PlayerControl.LocalPlayer.IsLover()) Utils.SetTarget(ref role.ClosestPlayer, killButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Impostors) && !x.IsCursed()).ToList());
            else if (!CustomGameOptions.MadmateKillEachOther) Utils.SetTarget(ref role.ClosestPlayer, killButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Impostors) && !x.Is(Faction.Madmates) && !x.IsCursed()).ToList());
            else Utils.SetTarget(ref role.ClosestPlayer, killButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Impostors) && !x.IsCursed()).ToList());

            role.SpellButton.SetCoolDown(role.KillCooldown, GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
            role.SpellButton.graphic.SetCooldownNormalizedUvs();

            var labelrender = role.SpellText;
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