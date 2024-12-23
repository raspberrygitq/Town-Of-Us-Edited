using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using System.Linq;
using TownOfUs.Extensions;
using TownOfUs.CrewmateRoles.SheriffMod;

namespace TownOfUs.ImpostorRoles.PoisonerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite PoisonSprite => TownOfUs.PoisonSprite;
        public static Sprite PoisonedSprite => TownOfUs.PoisonedSprite;

        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Poisoner)) return;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            var role = Role.GetRole<Poisoner>(PlayerControl.LocalPlayer);
            if (role.PoisonButton == null)
            {
                role.PoisonButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.PoisonButton.graphic.enabled = true;
                role.PoisonButton.graphic.sprite = PoisonSprite;
            }

            role.PoisonButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            var position = __instance.KillButton.transform.localPosition;
            role.PoisonButton.transform.localPosition = new Vector3(position.x,
                position.y, position.z);

            var notImp = PlayerControl.AllPlayerControls
                    .ToArray()
                    .Where(x => !x.Is(Faction.Impostors))
                    .ToList();
                    
            if (!role.PoisonButton.isActiveAndEnabled) role.ClosestPlayer = null;
            else if (!PlayerControl.LocalPlayer.moveable) role.ClosestPlayer = null;
            else if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayer, role.PoisonButton);
            else if (PlayerControl.LocalPlayer.IsLover() && CustomGameOptions.ImpLoverKillTeammate) Utils.SetTarget(ref role.ClosestPlayer, role.PoisonButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover()).ToList());
            else if (PlayerControl.LocalPlayer.IsLover() && (!CustomGameOptions.MadmateKillEachOther || CustomGameOptions.GameMode == GameMode.Cultist)) Utils.SetTarget(ref role.ClosestPlayer, role.PoisonButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Impostors) && !x.Is(Faction.Madmates)).ToList());
            else if (PlayerControl.LocalPlayer.IsLover()) Utils.SetTarget(ref role.ClosestPlayer, role.PoisonButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Impostors)).ToList());
            else if (!CustomGameOptions.MadmateKillEachOther || CustomGameOptions.GameMode == GameMode.Cultist) Utils.SetTarget(ref role.ClosestPlayer, role.PoisonButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Impostors) && !x.Is(Faction.Madmates)).ToList());
            else Utils.SetTarget(ref role.ClosestPlayer, role.PoisonButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Impostors)).ToList());

            if (role.ClosestPlayer != null)
            {
                role.ClosestPlayer.myRend().material.SetColor("_OutlineColor", Palette.Purple);
            }

            try
            {
                if (role.Poisoned)
                {
                    role.PoisonButton.graphic.sprite = PoisonedSprite;
                    role.Poison();
                    role.PoisonButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.PoisonDuration);
                }
                else
                {
                    role.PoisonButton.graphic.sprite = PoisonSprite;
                    if (role.PoisonedPlayer && role.PoisonedPlayer != PlayerControl.LocalPlayer)
                    {
                        role.PoisonKill();
                    }
                    if (role.ClosestPlayer != null)
                    {
                        role.PoisonButton.graphic.color = Palette.EnabledColor;
                        role.PoisonButton.graphic.material.SetFloat("_Desat", 0f);
                    }
                    else
                    {
                        role.PoisonButton.graphic.color = Palette.DisabledClear;
                        role.PoisonButton.graphic.material.SetFloat("_Desat", 1f);
                    }
                    role.PoisonButton.SetCoolDown(role.PoisonTimer(), CustomGameOptions.PoisonCD);
                    role.PoisonedPlayer = PlayerControl.LocalPlayer; //Only do this to stop repeatedly trying to re-kill poisoned player. null didn't work for some reason
                }
            }
            catch
            {

            }
        }
    }
}
