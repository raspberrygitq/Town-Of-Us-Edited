using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using TownOfUs.Extensions;
using System.Linq;
using AmongUs.GameOptions;

namespace TownOfUs.Patches.ImpostorRoles.ManipulatorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite ManipulateSprite => TownOfUs.ManipulateSprite;
        public static void Postfix(HudManager __instance)
        {
            var player = PlayerControl.LocalPlayer;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Manipulator)) return;

            var role = Role.GetRole<Manipulator>(PlayerControl.LocalPlayer);

            if (role.ManipulateButton == null)
            {
                role.ManipulateButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ManipulateButton.graphic.enabled = true;
                role.ManipulateButton.gameObject.SetActive(false);
                role.ManipulateButton.graphic.sprite = ManipulateSprite;
            }

             // Check if the game state allows the KillButton to be active
            bool isKillButtonActive = __instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled;
            isKillButtonActive = isKillButtonActive && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead;
            isKillButtonActive = isKillButtonActive && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started;

            role.ManipulateButton.gameObject.SetActive(isKillButtonActive);
            role.ManipulateButton.transform.localPosition = new Vector3(-2f, 1f, 0f);

            var notimp = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Impostors)).ToList();

            var killButton = role.ManipulateButton;
            if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayer, killButton);
            else if (PlayerControl.LocalPlayer.IsLover() && CustomGameOptions.ImpLoverKillTeammate) Utils.SetTarget(ref role.ClosestPlayer, killButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover()).ToList());
            else if (PlayerControl.LocalPlayer.IsLover() && !CustomGameOptions.MadmateKillEachOther) Utils.SetTarget(ref role.ClosestPlayer, killButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Impostors) && !x.Is(Faction.Madmates)).ToList());
            else if (PlayerControl.LocalPlayer.IsLover()) Utils.SetTarget(ref role.ClosestPlayer, killButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Impostors)).ToList());
            else if (!CustomGameOptions.MadmateKillEachOther) Utils.SetTarget(ref role.ClosestPlayer, killButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !x.Is(Faction.Impostors) && !x.Is(Faction.Madmates)).ToList());
            else Utils.SetTarget(ref role.ClosestPlayer, killButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Impostors)).ToList());

            if (role.ClosestPlayer != null)
            {
                role.ClosestPlayer.myRend().material.SetColor("_OutlineColor", Palette.ImpostorRed);
            }

            role.ManipulateButton.SetCoolDown(role.KillCooldown, GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
        }
    }
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class PatchManipulate
    {
        public static void Postfix(HudManager __instance)
        {
            var manipulators = Role.AllRoles.Where(x => x.RoleType == RoleEnum.Manipulator && x.Player != null).Cast<Manipulator>();
                foreach (var role in manipulators)
                {
                    if (!MeetingHud.Instance && role.ManipulatedPlayer != null && !role.ManipulatedPlayer.Data.IsDead && role.IsManipulating == true)
                    {
                    var notdead = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead).ToList();
                    var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
                    var playerToDie = Utils.GetClosestPlayer(role.ManipulatedPlayer, notdead);
                    if (playerToDie != null && !playerToDie.Is(Faction.Impostors) && Vector2.Distance(playerToDie.GetTruePosition(),
                    role.ManipulatedPlayer.GetTruePosition()) < maxDistance)
                    {
                        SoundManager.Instance.PlaySound(role.Player.KillSfx, false, 0.5f);
                        Utils.Interact(role.ManipulatedPlayer, playerToDie, true);
                        Utils.Rpc(CustomRPC.SetManipulateOff, role.Player.PlayerId);
                        role.ManipulatedPlayer = null;
                        role.IsManipulating = false;
                    }
                    }
                }
        }
    }
}